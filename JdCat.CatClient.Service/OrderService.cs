using JdCat.CatClient.Common;
using JdCat.CatClient.IService;
using JdCat.CatClient.Model;
using JdCat.CatClient.Model.Enum;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.Service
{

    public class OrderService : BaseRedisService, IOrderService
    {
        public OrderService(IConnectionMultiplexer connectionMultiplexer, DatabaseConfig config) : base(connectionMultiplexer, config)
        {
        }

        public void SaveOrder(TangOrder order)
        {
            order.Code = GenerateOrderCode();
            Save(order);
            // 保存到未完成订单列表
            var key = AddKeyPrefix<TangOrder>("UnFinish");
            Database.ListLeftPush(key, order.ObjectId);
        }

        public List<TangOrder> GetUnfinishOrder()
        {
            var key = AddKeyPrefix<TangOrder>("UnFinish");
            var unfinish = Database.ListRange(key);
            if (unfinish == null || unfinish.Length == 0) return null;
            var list = Get<TangOrder>(unfinish.Select(a => a.ToString()).ToArray());
            list.ForEach(order => {
                order.TangOrderProducts = GetOrderProduct(order.ObjectId).ToObservable();
            });
            return list;
        }

        public void SaveOrderProduct(TangOrderProduct product)
        {
            if (!string.IsNullOrEmpty(product.ObjectId))
            {
                UpdateOrderProduct(product);
                return;
            }
            product.ObjectId = Guid.NewGuid().ToString().ToLower();
            product.CreateTime = DateTime.Now;
            var key = AddKeyPrefix(product.ObjectId, typeof(TangOrderProduct).Name);
            Database.StringSet(key, JsonConvert.SerializeObject(product));
            // 保存到订单关联的产品列表中
            var orderProductKey = AddKeyPrefix($"Order:{product.OrderObjectId}", typeof(TangOrderProduct).Name);
            var list = Database.ListRange(orderProductKey);
            var exist = list?.Any(a => a.ToString() == product.ObjectId);
            if (!exist.HasValue || !exist.Value)
            {
                Database.ListRightPush(orderProductKey, product.ObjectId);
            }
        }

        public List<TangOrderProduct> GetOrderProduct(string objectId)
        {
            var orderProductKey = AddKeyPrefix($"Order:{objectId}", typeof(TangOrderProduct).Name);
            var ids = Database.ListRange(orderProductKey);
            var keys = ids.Select(a => (RedisKey)AddKeyPrefix(a, typeof(TangOrderProduct).Name)).ToArray();
            var vals = Database.StringGet(keys);
            var entitys = vals.Select(a => JsonConvert.DeserializeObject<TangOrderProduct>(a)).ToList();
            return entitys;
        }

        public void SubmitOrder(TangOrder order)
        {
            order.OrderStatus = TangOrderStatus.Eating;
            order.Identifier = (int)GenerateOrderIdentity();
            Update(order);
            foreach (var product in order.TangOrderProducts)
            {
                product.ProductStatus = TangOrderProductStatus.Ordered;
                Update(product);
            }
        }

        public void ReSubmitOrder(TangOrder order)
        {
            foreach (var product in order.TangOrderProducts.Where(a => a.ProductStatus == TangOrderProductStatus.Add))
            {
                product.ProductStatus = TangOrderProductStatus.Added;
                Update(product);
            }
        }

        public void DeleteOrder(TangOrder order)
        {
            /** 操作步骤
             *  1. 删除订单实体
             *  2. 删除订单列表项
             *  3. 删除未完成订单列表项
             *  4. 删除已点菜品实体
             *  5. 删除订单已点菜品列表
             *  6. 删除订单支付数据
             *  7. 删除订单支付列表
             */
            var orderKey = AddKeyPrefix<TangOrder>(order.ObjectId);
            Database.KeyDelete(orderKey);

            var orderListKey = AddKeyPrefix<TangOrder>("List");
            Database.ListRemove(orderListKey, order.ObjectId);

            var unFinishKey = AddKeyPrefix<TangOrder>("UnFinish");
            Database.ListRemove(unFinishKey, order.ObjectId);

            var orderProductListKey = AddKeyPrefix($"Order:{order.ObjectId}", typeof(TangOrderProduct).Name);
            var productList = Database.ListRange(orderProductListKey);
            if (productList.Length > 0)
            {
                foreach (var val in productList)
                {
                    var key = AddKeyPrefix(val, typeof(TangOrderProduct).Name);
                    Database.KeyDelete(key);
                }
            }
            Database.KeyDelete(orderProductListKey);

            var orderPaymentListKey = AddKeyPrefix($"TangOrder:{order.ObjectId}", typeof(TangOrderPayment).Name);
            var paymentListKey = AddKeyPrefix("List", typeof(TangOrderPayment).Name);
            var paymentList = Database.ListRange(orderPaymentListKey);
            if (paymentList.Length > 0)
            {
                foreach (var val in paymentList)
                {
                    var key = AddKeyPrefix(val, typeof(TangOrderPayment).Name);
                    Database.KeyDelete(key);
                    Database.ListRemove(paymentListKey, val);
                }
            }
            Database.KeyDelete(orderPaymentListKey);
        }

        public void UpdateOrderProduct(TangOrderProduct product)
        {
            if (product.Quantity <= 0)
            {
                var key = AddKeyPrefix(product.ObjectId, typeof(TangOrderProduct).Name);
                Database.KeyDelete(key);

                var orderProductListKey = AddKeyPrefix($"Order:{product.OrderObjectId}", typeof(TangOrderProduct).Name);
                Database.ListRemove(orderProductListKey, product.ObjectId);
            }
            else
            {
                Update(product);
            }
        }

        public async Task PaymentAsync(TangOrder order)
        {
            order.OrderStatus = TangOrderStatus.Settled;
            Update(order);
            // 保存支付方式
            var orderPaymentKey = AddKeyPrefix<TangOrderPayment>($"Order:{order.ObjectId}");
            order.TangOrderPayments.ForEach(payment => 
            {
                Save(payment);
            });
            await SetRelativeEntitysAsync<TangOrderPayment, TangOrder>(order.ObjectId, order.TangOrderPayments.ToArray());
            var key = AddKeyPrefix<TangOrder>("UnFinish");
            Database.ListRemove(key, order.ObjectId);
        }



        public TangOrderProduct Unsubscribe(TangOrder order, TangOrderProduct product, double quantity)
        {
            if (product.Quantity <= quantity)
            {
                product.ProductStatus = TangOrderProductStatus.Return;
                Update(product);
                return product;
            }
            product.Quantity = product.Quantity - quantity;
            product.Amount = product.Quantity * product.Price;
            Update(product);
            var retProduct = new TangOrderProduct
            {
                Description = product.Description,
                Discount = product.Discount,
                Feature = product.Feature,
                FormatId = product.FormatId,
                Name = product.Name,
                OrderId = product.OrderId,
                OrderObjectId = product.OrderObjectId,
                OriginalPrice = product.OriginalPrice,
                Price = product.Price,
                ProductId = product.ProductId,
                ProductIdSet = product.ProductIdSet,
                ProductStatus = TangOrderProductStatus.Return,
                Quantity = quantity,
                RefundReason = product.RefundReason,
                Remark = product.Remark,
                Amount = quantity * product.Price,
                Src = product.Src
            };
            Save(retProduct);
            var orderProductKey = AddKeyPrefix($"Order:{retProduct.OrderObjectId}", typeof(TangOrderProduct).Name);
            Database.ListRightPush(orderProductKey, retProduct.ObjectId);
            order.TangOrderProducts.Add(retProduct);
            return retProduct;
        }

        public async Task FenOrderAsync(TangOrderProduct good, TangOrder originalOrder, TangOrder targetOrder)
        {
            good.OrderId = targetOrder.Id;
            good.OrderObjectId = targetOrder.ObjectId;
            originalOrder.TangOrderProducts.Remove(good);
            if(targetOrder.TangOrderProducts == null)
            {
                targetOrder.TangOrderProducts = new ObservableCollection<TangOrderProduct>();
            }
            targetOrder.TangOrderProducts.Add(good);

            var originalKey = AddKeyPrefix($"Order:{originalOrder.ObjectId}", typeof(TangOrderProduct).Name);
            await Database.ListRemoveAsync(originalKey, good.ObjectId);

            var targetKey = AddKeyPrefix($"Order:{targetOrder.ObjectId}", typeof(TangOrderProduct).Name);
            await Database.ListRightPushAsync(targetKey, good.ObjectId);

        }

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns></returns>
        private string GenerateOrderCode()
        {
            var key = $"{DatabaseConfig.KeyPrefix}:OrderCode:{DateTime.Now:yyyyMMdd}";
            var codeflow = Database.StringIncrement(key).ToString().PadLeft(5, '0');

            var code = string.Empty;
            var random = new Random();
            for (var i = 0; i < 4; i++)
            {
                var index = random.Next(0, 9);
                code += index;
            }
            code = $"{DateTime.Now:yyyyMMddHHmmss}{codeflow}{code}";
            return code;
        }

        /// <summary>
        /// 获取当日订单流水
        /// </summary>
        /// <returns></returns>
        private long GenerateOrderIdentity()
        {
            var key = $"{DatabaseConfig.KeyPrefix}:OrderIdentity:{DateTime.Now:yyyyMMdd}";
            return Database.StringIncrement(key);
        }

    }
}
