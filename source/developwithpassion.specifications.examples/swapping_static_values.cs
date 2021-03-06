using System.Security;
using System.Security.Principal;
using System.Threading;
using developwithpassion.specifications.extensions;
using developwithpassion.specifications.rhinomocks;
using Machine.Specifications;

namespace developwithpassion.specifications.examples
{
    public class swapping_static_values
    {
        [Subject(typeof(Calculator))]
        public class when_shutting_off_the_calculator_and_they_are_not_in_the_correct_security_role :
            Observes<Calculator>
        {
            Establish c = () =>
            {
                fake_principal = fake.an<IPrincipal>();

                fake_principal.setup(x => x.IsInRole("blah")).Return(false);
                //The change method is what allows you to swap the value of a static property or
                //field solely for the duration of a test. After the test completes it will be
                //reset back to its original value
                spec.change(() => Thread.CurrentPrincipal).to(fake_principal);
            };

            Because b = () =>
                spec.catch_exception(() => sut.shut_off());

            It should_throw_a_security_exception = () =>
                spec.exception_thrown.ShouldBeAn<SecurityException>();

            static IPrincipal fake_principal;
        }

        public class Calculator
        {
            public void shut_off()
            {
                if (Thread.CurrentPrincipal.IsInRole("blah")) return;
                throw new SecurityException();
            }
        }
    }
}