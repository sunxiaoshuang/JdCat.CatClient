using JdCat.CatClient.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdCat.CatClient.IService
{
    public interface IOrderService : IBaseService
    {
        /// <summary>
        /// 保存订单
        /// </summary>
        /// <param name="order"></param>
        void SaveOrder(TangOrder order);
        /// <summary>
        /// 获取所有未完成的订单
        /// </summary>
        /// <returns></returns>
        List<TangOrder> GetUnfinishOrder();
        /// <summary>
        /// 保存订单产品
        /// </summary>
        /// <param name="product"></param>
        void SaveOrderProduct(TangOrderProduct product);
        /// <summary>
        /// 根据订单id获取订单产品
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        List<TangOrderProduct> GetOrderProduct(string objectId);
        /// <summary>
        /// 下单
        /// </summary>
        /// <param name="order"></param>
        void SubmitOrder(TangOrder order);
        /// <summary>
        /// 加菜
        /// </summary>
        /// <param name="order"></param>
        void ReSubmitOrder(TangOrder order);
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="order"></param>
        void DeleteOrder(TangOrder order);
        /// <summary>
        /// 更新订单产品
        /// </summary>
        /// <param name="product"></param>
        void UpdateOrderProduct(TangOrderProduct product);
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="order"></param>
        Task PaymentAsync(TangOrder order);
        /// <summary>
        /// 退菜
        /// </summary>
        /// <param name="order"></param>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        TangOrderProduct Unsubscribe(TangOrder order, TangOrderProduct product, double quantity);
        /// <summary>
        /// 分单
        /// </summary>
        /// <param name="good">商品</param>
        /// <param name="originalOrder">原订单</param>
        /// <param name="targetOrder">目标订单</param>
        /// <returns></returns>
        Task FenOrderAsync(TangOrderProduct good, TangOrder originalOrder, TangOrder targetOrder);
        /// <summary>
        /// 保存快餐订单
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task SaveFastOrderAsync(TangOrder order);
        /// <summary>
        /// 载入快餐正在点餐的订单
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Task<TangOrder> GetFastOrderAsync();
        /// <summary>
        /// 完成快餐订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<TangOrder> FinishFastOrderAsync(TangOrder order);

    }
}
