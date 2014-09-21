using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;

namespace DevUpdater.Server
{
    public class HashModelBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(Hash))
                return false;
            
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value == null)
                return false;

            string valueStr = value.RawValue as string;
            if (valueStr == null)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Wrong value type.");
                return false;
            }

            Hash result;
            try
            {
                result = Hash.Parse(valueStr);
            }
            catch
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Cannot parse hash.");
                return false;
            }
            bindingContext.Model = result;
            return true;
        }
    }
}
