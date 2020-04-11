﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    [Route("/")]
    [Route("/{Year:int}/{Month:int}")]
    [Authorize]
    public class SummaryMonth : Summary<MonthModel>
    {
        [Parameter]
        public int? Year { get; set; }

        [Parameter]
        public int? Month { get; set; }

        public SummaryMonth() 
            => SubTitle = "Per-month summary of expenses in categories";

        protected override void ClearPreviousParameters()
        {
            Year = null;
            Month = null;
        }

        protected override MonthModel CreateSelectedItemFromParameters()
        {
            Log.Debug($"CreateSelectedItemFromParameters(Year='{Year}', Month='{Month}')");

            if (Year != null && Month != null)
                return new MonthModel(Year.Value, Month.Value);
            else
                return null;
        }

        protected override IQuery<List<MonthModel>> CreatePeriodsQuery()
            => new ListMonthWithOutcome();

        protected override IQuery<Price> CreateTotalQuery(MonthModel item)
            => new GetTotalMonthOutcome(item);

        protected override IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(MonthModel item)
            => new ListMonthCategoryWithOutcome(item);

        protected override bool IsContained(DateTime changed)
            => Periods.Contains(changed);

        protected override string UrlSummary(MonthModel item)
            => Navigator.UrlSummary(item);

        protected override void OpenOverview(MonthModel item)
            => Navigator.OpenOverview(item);

        protected override void OpenOverview(MonthModel item, IKey categorykey)
            => Navigator.OpenOverview(item, categorykey);
    }
}
