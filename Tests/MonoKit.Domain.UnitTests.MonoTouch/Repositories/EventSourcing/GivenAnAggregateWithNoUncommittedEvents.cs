
using System;
using NUnit.Framework;
using MonoKit.Data;

namespace MonoKit.Domain.UnitTests.Repositories.EventSourcing
{
    [TestFixture]
    public class GivenAnAggregateWithNoUncommittedEvents : GivenAnEventSourcedAggregateRepository
    {
        public TestAggregateRoot Aggregate { get; private set; }
        
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            this.Aggregate = new TestAggregateRoot();
        }
        
        [Test]
        public void WhenSavingTheAggregate_ThenTheSaveResultIsNone()
        {
            var result = this.Repository.Save(this.Aggregate);
            Assert.AreEqual(SaveResult.None, result);
        }
    }
}
