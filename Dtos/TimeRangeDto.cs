using System;

namespace UrlService.Dtos
{
    public class TimeRangeDto
    {
        public TimeRangeDto(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;

        }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

    }
    
}