﻿using System.Linq;
using FizzWare.NBuilder;
using HiringManager.EntityModel;
using NSubstitute;
using NUnit.Framework;
using Simple.Validation;

namespace HiringManager.DomainServices.Transactions.UnitTests
{
    [TestFixture]
    public class CloseTemplateTests
    {
        [TestFixtureSetUp]
        public void BeforeAnyTestRuns()
        {
        }

        [SetUp]
        public void BeforeEachTestRuns()
        {
            this.DbContext = Substitute.For<IDbContext>();
            this.ValidationEngine = Substitute.For<IValidationEngine>();
            this.Transaction = new ClosePosition(this.DbContext, this.ValidationEngine);
        }

        public IValidationEngine ValidationEngine { get; set; }

        public IDbContext DbContext { get; set; }

        public ClosePosition Transaction { get; set; }

        [Test]
        public void Execute()
        {
            // Arrange
            var position = Builder<Position>
                .CreateNew()
                .Build()
                ;

            this.DbContext.Get<Position>(position.PositionId.Value).Returns(position);

            // Act
            var response = this.Transaction.Execute(position.PositionId.Value);

            // Assert
            Assert.That(response, Is.Not.Null);
            this.DbContext.Received().SaveChanges();

            Assert.That(position.Status, Is.EqualTo("Closed"));
        }

        [Test]
        public void Execute_WhenPositionHasCandidates()
        {
            // Arrange
            var statuses = Builder<CandidateStatus>
                .CreateListOfSize(3)
                .Build()
                .ToList()
                ;
            var position = Builder<Position>
                .CreateNew()
                .Do(row => row.Candidates = statuses)
                .Build()
                ;

            this.DbContext.Get<Position>(position.PositionId.Value).Returns(position);

            // Act
            var response = this.Transaction.Execute(position.PositionId.Value);

            // Assert
            foreach (var status in statuses)
            {
                Assert.That(status.Status, Is.EqualTo("Passed"));
            }

        }

        [Test]
        public void Execute_WithValidationErrors()
        {
            // Arrange
            var statuses = Builder<CandidateStatus>
                .CreateListOfSize(3)
                .Build()
                .ToList()
                ;
            var position = Builder<Position>
                .CreateNew()
                .Do(row => row.Candidates = statuses)
                .Build()
                ;

            var validationResults = new[]
                                    {
                                        new ValidationResult(),
                                    };
            this.ValidationEngine.Validate(position, "Close").Returns(validationResults);

            this.DbContext.Get<Position>(position.PositionId.Value).Returns(position);

            // Act
            var response = this.Transaction.Execute(position.PositionId.Value);

            // Assert
            Assert.That(response.ValidationResults, Is.EquivalentTo(validationResults));
            this.DbContext.DidNotReceive().Add(position);
            this.DbContext.DidNotReceive().SaveChanges();
        }
    }
}
