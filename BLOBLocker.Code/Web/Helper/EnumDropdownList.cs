﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BLOBLocker.Code.Web.Helper
{
    public static class EnumDropDownList
    {
        public static HtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, string firstElement)
        {
            var typeOfProperty = modelExpression.ReturnType;
            if (!typeOfProperty.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));
            var enumValues = new SelectList(Enum.GetValues(typeOfProperty));
            htmlHelper.Hidden("baw");
            return htmlHelper.DropDownListFor(modelExpression, enumValues, firstElement);
        }
    }
}
