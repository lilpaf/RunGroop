using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Controllers;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.Services;
using RunGroopWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RunGroopWebApp.Tests.Controller
{
    public class ClubControllerTests
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly ClubController _clubController;

        public ClubControllerTests()
        {
            // Dependencies
            _clubRepository = A.Fake<IClubRepository>();
            _photoService = A.Fake<IPhotoService>();

            // SUT
            _clubController = new(_clubRepository, _photoService);
        }

        [Fact]
        public async Task ClubController_Index_ReturnsSuccess()
        {
            // Arrange
            //var clubs = new List<Club> { new Club(), new Club() };
            var clubs = A.Fake<IEnumerable<Club>>();

            A.CallTo(() => _clubRepository.GetSliceAsync(A<int>.Ignored, A<int>.Ignored)).Returns(clubs);
            A.CallTo(() => _clubRepository.GetCountAsync()).Returns(10);

            // Act
            var result = await _clubController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<IndexClubViewModel>().Subject;

            model.Clubs.Should().BeEquivalentTo(clubs);
            model.Page.Should().Be(1);
            model.PageSize.Should().Be(6);
            model.TotalClubs.Should().Be(10);
            model.TotalPages.Should().Be(2);  // 10 clubs, 6 per page -> 2 pages
            model.Category.Should().Be(-1);
        }

        [Fact]
        public void ClubController_DetailClub_ReturnsSuccess()
        {
            // Arrange
            int id = 1;
            string runningClub = "Test";
            Club club = A.Fake<Club>();
            A.CallTo(() => _clubRepository.GetByIdAsync(id)).Returns(club);

            // Act
            var result = _clubController.DetailClub(id, runningClub);

            // Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public async Task ClubController_DetailClub_ReturnsNotFound()
        {
            // Arrange
            int id = 1;
            string runningClub = "Test";
            A.CallTo(() => _clubRepository.GetByIdAsync(id)).Returns(Task.FromResult<Club?>(null));

            // Act
            var result = await _clubController.DetailClub(id, runningClub);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
