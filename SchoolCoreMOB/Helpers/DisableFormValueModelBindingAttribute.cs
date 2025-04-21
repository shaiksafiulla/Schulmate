using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SchoolCoreMOB.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var factories = context.ValueProviderFactories;
            factories.RemoveType<FormValueProviderFactory>();
            factories.RemoveType<FormFileValueProviderFactory>();
            factories.RemoveType<JQueryFormValueProviderFactory>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
    //public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
    //{
    //    public void OnResourceExecuting(ResourceExecutingContext context)
    //    {
    //        var formValueProviderFactory = context.ValueProviderFactories
    //            .OfType<FormValueProviderFactory>()
    //            .FirstOrDefault();
    //        if (formValueProviderFactory != null)
    //        {
    //            context.ValueProviderFactories.Remove(formValueProviderFactory);
    //        }

    //        var jqueryFormValueProviderFactory = context.ValueProviderFactories
    //            .OfType<JQueryFormValueProviderFactory>()
    //            .FirstOrDefault();
    //        if (jqueryFormValueProviderFactory != null)
    //        {
    //            context.ValueProviderFactories.Remove(jqueryFormValueProviderFactory);
    //        }
    //    }

    //    public void OnResourceExecuted(ResourceExecutedContext context)
    //    {
    //    }
    //}
}
