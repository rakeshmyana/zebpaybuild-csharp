using System.Collections.Generic;
using System.Threading.Tasks;
using Zebpay.Model;
using Zebpay.Model.Request;
using Zebpay.Model.Response;

namespace Zebpay.RestClient
{
    internal interface ITradeService
    {
        Task<ResponseModel<OrderResponse>> Create(OrderRequest createOrderRequest);
        Task<ResponseModel<IList<Order>>> Orders(string tradePair = null, int orderId = 0, string status = "all", int page = 1, int limit = 20);
        Task<ResponseModel<string>> Cancel(int orderId);
        Task<ResponseModel<IList<Balance>>> Balance(string tradePair);
        Task<ResponseModel<IList<Fills>>> Fills(int orderId);  
    }
}
