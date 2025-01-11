using Autofac.Core;
using Serilog;

namespace WPFUiDesktopApp
{
    /// <summary>
    /// Custom registration source to log missing registrations.
    /// </summary>
    internal class MissingRegistrationLogger : IRegistrationSource
    {
        public bool IsAdapterForIndividualComponents => false;

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            var existingRegistrations = registrationAccessor(service);
            if (!existingRegistrations.Any())
            {
                var logger = Log.Logger;
                logger.Warning("Type {Type} is not registered in the container.", service.Description);
            }

            return Enumerable.Empty<IComponentRegistration>();
        }
    }
}