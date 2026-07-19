using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.ExcelProcessor.CustomTemplate;

namespace FBSC.ODMS.ExcelProcessor.CustomProcessor
{
    /// <summary>
    /// This is a sample only for implementing batch upload with custom template
    /// </summary>
    public static class SampleTableCustomProcessor
    {
        public async static Task Process(ApplicationContext context, IList<SampleTableState> listOfData, string userId)
        {
            foreach (var data in listOfData)
            {           
                //Add custom logic here for custom processing of data
                await context.AddAsync(data);
                await context.UpdateBatchRecordAsync(userId, data);
            }
        }
    }
}
