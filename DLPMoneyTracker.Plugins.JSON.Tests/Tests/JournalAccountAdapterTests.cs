using DLPMoneyTracker.Plugins.JSON.Tests.Fixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Plugins.JSON.Tests.Tests
{
    public class JournalAccountAdapterTests : IClassFixture<JSONFixture>
    {
        private readonly JSONFixture _fixture;

        public JournalAccountAdapterTests(JSONFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void Adapter_ShouldSaveNewAccount()
        {

        }
    }
}
