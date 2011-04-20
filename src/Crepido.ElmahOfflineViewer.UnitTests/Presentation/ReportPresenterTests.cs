﻿using System;
using System.Collections.Generic;
using Crepido.ElmahOfflineViewer.Core.Common;
using Crepido.ElmahOfflineViewer.Core.Domain;
using Crepido.ElmahOfflineViewer.Core.Domain.Abstract;
using Crepido.ElmahOfflineViewer.Core.Presentation;
using Crepido.ElmahOfflineViewer.Core.Presentation.Abstract;
using Moq;
using NUnit.Framework;

namespace Crepido.ElmahOfflineViewer.UnitTests.Presentation
{
	[TestFixture]
	public class ReportPresenterTests : UnitTestBase
	{
		[Test]
		public void Ctor_SetsView()
		{
			// arrange
			var view = new Mock<IReportView>();
			var generator = new Mock<IReportGenerator>();

			// act
			var presenter = new ReportPresenter(view.Object, generator.Object);
			
			// assert
			Assert.That(presenter.View, Is.EqualTo(view.Object));
		}

		[Test]
		public void ViewOnLoaded_SetsDefaultTimeInterval()
		{
			// arrange
			var view = new Mock<IReportView>();
			var generator = new Mock<IReportGenerator>();
			var presenter = new ReportPresenter(view.Object, generator.Object);
			var expectedInterval = new DateInterval(DateTime.Today.AddDays(-7), DateTime.Today);

			// act
			view.Raise(x => x.OnLoaded += null, new EventArgs());

			// assert
			view.Verify(x => x.SetTimeInterval(It.Is<DateInterval>(y => y.Equals(expectedInterval))), Times.Once());
		}
		
		[Test]
		public void ViewOnLoaded_LoadReportTypes()
		{
			// arrange
			var view = new Mock<IReportView>();
			var generator = new Mock<IReportGenerator>();
			var presenter = new ReportPresenter(view.Object, generator.Object);

			// act
			view.Raise(x => x.OnLoaded += null, new EventArgs());

			// assert
			view.Verify(x => x.LoadReportTypes(It.IsAny<List<ReportTypeListItem>>()), Times.Once());
		}

		[Test]
		public void ViewOnReportSelected_DisplaysReport()
		{
			// arrange
			var view = new Mock<IReportView>();
			var generator = new Mock<IReportGenerator>();

			var presenter = new ReportPresenter(view.Object, generator.Object);
			var reporyQuery = CreateReportQuery();
			var args = new ReportSelectionEventArgs(reporyQuery);

			var report = new Report(reporyQuery);
			generator.Setup(x => x.Create(reporyQuery)).Returns(report);

			// act
			view.Raise(x => x.OnReportSelected += null, args);

			// assert
			view.Verify(x => x.DisplayReport(report), Times.Once());
		}

		[Test]
		public void GeneratorOnDataSourceInitialized_ClearsView()
		{
			// arrange
			var view = new Mock<IReportView>();
			var generator = new Mock<IReportGenerator>();
			var presenter = new ReportPresenter(view.Object, generator.Object);

			// act
			generator.Raise(x => x.OnDataSourceInitialized += null, new EventArgs());

			// assert
			view.Verify(x => x.Clear(), Times.Once());
		}
	}
}
