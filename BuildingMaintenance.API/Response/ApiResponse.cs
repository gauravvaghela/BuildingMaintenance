using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingMaintenance.API.Response
{
    public class ApiResponse<T> where T : class
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public T Data { get; set; }
        public IList<T> DataList { get; set; }

        public ApiResponse(string status, string mesage)
        {
            Message = mesage;
            Status = status;
        }

        public ApiResponse(string status, string mesage, T data)
        {
            Message = mesage;
            Status = status;
            Data = data;
        }

        public ApiResponse(string status, string mesage, List<T> dataList)
        {
            Message = mesage;
            Status = status;
            DataList = dataList;
        }

    }
}
