using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Commander.Models;

namespace Commander.Common{
    public static class MyQuery
    {
        public static IEnumerable<VClassDetail> FilterDevices(IEnumerable<VClassDetail> devices, IEnumerable<Func<VClassDetail, string>> filters, string filterValue)
{
            foreach (var filter in filters)
            {
                devices = devices.Where(d => filter(d).Equals(filterValue));
            }

            return devices;
        }


        public static IQueryable<VClassDetail> ApplyFilter(IQueryable<VClassDetail> query, string property, long projectId, long batchId, string hostId)
        {
        switch (property)
        {
            case "ProjectId":
            return query.Where(p => p.ProjectId == projectId);
            case "BatchId":
            return query.Where(p => p.BatchId >= batchId);
            case "HostId":
            return query.Where(p => p.HostId == hostId);
            default:
            return query;
        }
        }
    }        
}