﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FizzWare.NBuilder;
using HiringManager.DomainServices;
using HiringManager.Mappers;
using HiringManager.Web.Controllers;
using HiringManager.Web.Models;
using HiringManager.Web.Models.Positions;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace HiringManager.Web.UnitTests.Controllers
{
    [TestFixture]
    public class PositionsControllerTests
    {
        [TestFixtureSetUp]
        public void BeforeAnyTestRuns()
        {

        }

        [SetUp]
        public void BeforeEachTestRuns()
        {
            this.Clock = Substitute.For<IClock>();

            this.PositionService = Substitute.For<IPositionService>();
            this.FluentMapper = Substitute.For<IFluentMapper>();
            this.PositionsController = new PositionsController(this.PositionService, this.FluentMapper, this.Clock);
        }

        public IClock Clock { get; set; }

        public IFluentMapper FluentMapper { get; set; }

        public IPositionService PositionService { get; set; }

        public PositionsController PositionsController { get; set; }

        [Test]
        public void Index()
        {
            // Arrange
            var response = new QueryResponse<PositionSummary>();
            this.PositionService.Query(Arg.Is<QueryPositionSummariesRequest>(arg => arg.Statuses == null ))
                .Returns(response)
                ;

            var indexViewModel = new IndexViewModel<PositionSummaryIndexItem>();
            this.FluentMapper
                .Map<IndexViewModel<PositionSummaryIndexItem>>()
                .From(response)
                .Returns(indexViewModel)
                ;

            // Act
            var viewResult = this.PositionsController.Index("");

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(indexViewModel));
        }

        [Test]
        public void Index_WithStatus()
        {
            // Arrange
            var response = new QueryResponse<PositionSummary>();
            this.PositionService.Query(Arg.Is<QueryPositionSummariesRequest>(arg => arg.Statuses.Contains("Open")))
                .Returns(response)
                ;

            var indexViewModel = new IndexViewModel<PositionSummaryIndexItem>();
            this.FluentMapper
                .Map<IndexViewModel<PositionSummaryIndexItem>>()
                .From(response)
                .Returns(indexViewModel)
                ;

            // Act
            var viewResult = this.PositionsController.Index("Open");

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(indexViewModel));
        }

        [Test]
        public void Create_HttpGet()
        {
            // Arrange
            this.Clock.Today.Returns(DateTime.Today);

            // Act
            var result = this.PositionsController.Create();

            // Assert
            var viewModel = result.Model as CreatePositionViewModel;
            Assert.That(viewModel, Is.Not.Null);
            Assert.That(viewModel.OpenDate, Is.EqualTo(this.Clock.Today));
        }

        [Test]
        public void Create_HttpPost()
        {
            // Arrange
            var viewModel = new CreatePositionViewModel();
            var request = new CreatePositionRequest();

            this.FluentMapper
                .Map<CreatePositionRequest>()
                .From(viewModel)
                .Returns(request)
                ;

            // Act
            var result = this.PositionsController.Create(viewModel);

            // Assert
            this.PositionService
                .Received()
                .CreatePosition(request)
                ;
        }

        [Test]
        public void Candidates()
        {
            // Arrange
            var positionDetails = new PositionDetails();
            var viewModel = new PositionCandidatesViewModel();

            this.PositionService.Details(1).Returns(positionDetails);

            this.FluentMapper
                .Map<PositionCandidatesViewModel>()
                .From(positionDetails)
                .Returns(viewModel)
                ;

            this.PositionService.Details(1).Returns(positionDetails);

            // Act
            var viewResult = this.PositionsController.Candidates(1);

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
        }

        [Test]
        public void AddCandidate_HttpGet()
        {
            // Arrange

            // Act
            var viewResult = this.PositionsController.AddCandidate(1);

            // Assert
            var viewModel = viewResult.Model as AddCandidateViewModel;
            Assert.That(viewModel.PositionId, Is.EqualTo(1));
        }

        [Test]
        public void AddCandidate_HttpPost()
        {
            // Arrange
            var viewModel = new AddCandidateViewModel();

            var request = new AddCandidateRequest();
            this.FluentMapper
                .Map<AddCandidateRequest>()
                .From(viewModel)
                .Returns(request)
                ;

            var response = Builder<AddCandidateResponse>
                .CreateNew()
                .Build();

            this.PositionService.AddCandidate(request).Returns(response);

            // Act
            var actionResult = this.PositionsController.AddCandidate(viewModel);

            // Assert
            var redirectToAction = actionResult as RedirectToRouteResult;
            Assert.That(redirectToAction, Is.Not.Null);

            Assert.That(redirectToAction.RouteName, Is.EqualTo(""));

            Assert.That(redirectToAction.RouteValues.ContainsKey("action"), Is.True);
            Assert.That(redirectToAction.RouteValues["action"], Is.EqualTo("Candidates"));

            Assert.That(redirectToAction.RouteValues.ContainsKey("id"), Is.True);
            Assert.That(redirectToAction.RouteValues["id"], Is.EqualTo(response.PositionId));
        }

        [Test]
        public void PassOnCandidate_HttpGet()
        {
            // Arrange
            var candidateStatusDetails = new CandidateStatusDetails();
            this.PositionService.GetCandidateStatusDetails(1)
                .Returns(candidateStatusDetails);

            var viewModel = new CandidateStatusViewModel();
            this.FluentMapper.Map<CandidateStatusViewModel>()
                .From(candidateStatusDetails)
                .Returns(viewModel)
                ;

            // Act
            var viewResult = this.PositionsController.Pass(1);

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
        }

        [Test]
        public void PassOnCandidate_HttpPost()
        {
            // Arrange
            var model = Builder<CandidateStatusViewModel>
                .CreateNew()
                .Build()
                ;

            // Act
            var result = this.PositionsController.Pass(model) as RedirectToRouteResult;

            // Assert
            Assert.That(result.RouteName, Is.EqualTo(""));

            Assert.That(result.RouteValues.ContainsKey("action"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Candidates"));

            Assert.That(result.RouteValues.ContainsKey("id"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(model.PositionId));

            this.PositionService
                .Received()
                .SetCandidateStatus(model.CandidateStatusId, "Passed");
        }

        [Test]
        public void Status_HttpGet()
        {
            // Arrange
            var candidateStatusDetails = new CandidateStatusDetails();
            this.PositionService.GetCandidateStatusDetails(1)
                .Returns(candidateStatusDetails);

            var viewModel = new CandidateStatusViewModel();
            this.FluentMapper.Map<CandidateStatusViewModel>()
                .From(candidateStatusDetails)
                .Returns(viewModel)
                ;

            // Act
            var viewResult = this.PositionsController.Status(1);

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
        }

        [Test]
        public void Status_HttpPost()
        {
            // Arrange
            var model = Builder<CandidateStatusViewModel>
                .CreateNew()
                .Build()
                ;

            // Act
            var result = this.PositionsController.Status(model) as RedirectToRouteResult;

            // Assert
            Assert.That(result.RouteName, Is.EqualTo(""));

            Assert.That(result.RouteValues.ContainsKey("action"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Candidates"));

            Assert.That(result.RouteValues.ContainsKey("id"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(model.PositionId));

            this.PositionService
                .Received()
                .SetCandidateStatus(model.CandidateStatusId, model.Status);
        }

        [Test]
        public void Hire_HttpGet()
        {
            // Arrange
            var candidateStatusDetails = new CandidateStatusDetails();
            this.PositionService.GetCandidateStatusDetails(1)
                .Returns(candidateStatusDetails);

            var viewModel = new CandidateStatusViewModel();
            this.FluentMapper.Map<CandidateStatusViewModel>()
                .From(candidateStatusDetails)
                .Returns(viewModel)
                ;

            // Act
            var viewResult = this.PositionsController.Hire(1);

            // Assert
            Assert.That(viewResult.Model, Is.SameAs(viewModel));
        }

        [Test]
        public void Hire_HttpPost()
        {
            // Arrange
            var model = Builder<CandidateStatusViewModel>
                .CreateNew()
                .Build()
                ;

            // Act
            var result = this.PositionsController.Hire(model) as RedirectToRouteResult;

            // Assert
            Assert.That(result.RouteName, Is.EqualTo(""));

            Assert.That(result.RouteValues.ContainsKey("action"));
            Assert.That(result.RouteValues["action"], Is.EqualTo("Candidates"));

            Assert.That(result.RouteValues.ContainsKey("id"));
            Assert.That(result.RouteValues["id"], Is.EqualTo(model.PositionId));

            this.PositionService
                .Received()
                .Hire(model.CandidateStatusId);
        }

    }
}
