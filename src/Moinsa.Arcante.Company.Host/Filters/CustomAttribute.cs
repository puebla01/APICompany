namespace API.Company.Host.Filters
{
    public class CustomAttribute
    {
        public readonly bool ConstainAttribute;
        public readonly bool Mandatory;

        public CustomAttribute(bool constainAttribute, bool mandatory)
        {
            ConstainAttribute = constainAttribute;
            Mandatory=mandatory;
        }
    }
}
