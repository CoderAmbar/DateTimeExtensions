﻿#region License

// 
// Copyright (c) 2011-2012, João Matos Silva <kappy@acydburne.com.pt>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DateTimeExtensions.Common;
using DateTimeExtensions.WorkingDays.OccurrencesCalculators;

namespace DateTimeExtensions.WorkingDays.CultureStrategies
{
    [Locale("fr-CA")]
    [Locale("en-CA")]
    public class FR_CAHolidayStrategy : HolidayStrategyBase, IHolidayStrategy
    {
        public FR_CAHolidayStrategy()
        {
            this.InnerCalendarDays.Add(new Holiday(GlobalHolidays.NewYear));
            this.InnerCalendarDays.Add(new Holiday(ChristianHolidays.GoodFriday));
            this.InnerCalendarDays.Add(new Holiday(ChristianHolidays.EasterMonday));
            //Victoria Day is not really national
            //this.InnerHolidays.Add(VictoriaDay);
            this.InnerCalendarDays.Add(new Holiday(CanadaDay));
            this.InnerCalendarDays.Add(new Holiday(LabourDay));
            this.InnerCalendarDays.Add(new Holiday(Thanksgiving));
            this.InnerCalendarDays.Add(new Holiday(RemembranceDay));
            this.InnerCalendarDays.Add(new Holiday(ChristianHolidays.Christmas));
            //Boxing is not really national
            //this.InnerHolidays.Add(GlobalHolidays.BoxingDay);
        }

        protected override IDictionary<DateTime, CalendarDay> BuildObservancesMap(int year)
        {
            IDictionary<DateTime, CalendarDay> holidayMap = new Dictionary<DateTime, CalendarDay>();
            foreach (var innerHoliday in InnerCalendarDays)
            {
                var date = innerHoliday.Day.GetInstance(year);
                if (date.HasValue)
                {
                    holidayMap.Add(date.Value, innerHoliday);
                    //if the holiday is a saturday, the holiday is observed on previous friday
                    if (date.Value.DayOfWeek == DayOfWeek.Saturday)
                    {
                        holidayMap.Add(date.Value.AddDays(2), innerHoliday);
                    }
                    //if the holiday is a sunday, the holiday is observed on next monday
                    if (date.Value.DayOfWeek == DayOfWeek.Sunday)
                    {
                        holidayMap.Add(date.Value.AddDays(1), innerHoliday);
                    }
                }
            }
            return holidayMap;
        }

        //First of July - Canada Day
        public static NamedDayInitializer CanadaDay { get; } = new NamedDayInitializer(() => 
            new NamedDay("Canada Day", new FixedDayStrategy(Month.July, 1)));

        //First Monday in September - Labour Day
        public static NamedDayInitializer LabourDay { get; } = new NamedDayInitializer(() => 
            new NamedDay("Labour Day", new NthDayOfWeekInMonthDayStrategy(1, DayOfWeek.Monday, Month.September, CountDirection.FromFirst)));
        
        public static NamedDayInitializer VictoriaDay { get; } = new NamedDayInitializer(() => 
            new NamedDay("Victoria Day", new NthDayOfWeekAfterDayStrategy(-1, DayOfWeek.Monday, new FixedDayStrategy(Month.May, 25))));

        //Second Monday in October - Thanksgiving
        public static NamedDayInitializer Thanksgiving { get; } = new NamedDayInitializer(() => 
            new NamedDay("Thanksgiving", new NthDayOfWeekInMonthDayStrategy(2, DayOfWeek.Monday, Month.October, CountDirection.FromFirst)));

        public static NamedDayInitializer RemembranceDay { get; } = new NamedDayInitializer(() => 
            new NamedDay("Remembrance Day", new FixedDayStrategy(Month.November, 11)));
    }
}