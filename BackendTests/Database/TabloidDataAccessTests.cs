using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class TabloidDataAccessTests
{
   private ViaTabloidDbContext CreateContext()
   {
       var options = new DbContextOptionsBuilder<ViaTabloidDbContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Isolated DB per test
           .Options;
       return new ViaTabloidDbContext(options);
   }


   [Fact]
   public async Task CreateStoryAsync_ShouldCreateNewStory_WhenValid()
   {
       using var context = CreateContext();
       var dataAccess = new TabloidDataAccess(context);


       var story = new Story { Title = "Test", Content = "Test content", DepartmentId = 1 };
       var result = await dataAccess.CreateStoryAsync(story);


       result.Should().NotBeNull();
       result.Title.Should().Be("Test");
   }


   [Fact]
   public async Task CreateStoryAsync_ShouldReturnExisting_WhenDuplicate()
   {
       using var context = CreateContext();
       var existing = new Story { Title = "Test", Content = "Old", DepartmentId = 1 };
       context.Stories.Add(existing);
       context.SaveChanges();


       var dataAccess = new TabloidDataAccess(context);
       var newStory = new Story { Title = "Test", Content = "New", DepartmentId = 1 };


       var result = await dataAccess.CreateStoryAsync(newStory);


       result.Id.Should().Be(existing.Id); // Same story
       context.Stories.CountAsync().Result.Should().Be(1);
   }


   [Fact]
   public async Task CreateStoryAsync_ShouldThrow_WhenTitleIsEmpty()
   {
       using var context = CreateContext();
       var dataAccess = new TabloidDataAccess(context);


       var invalidStory = new Story { Title = "", Content = "Bad", DepartmentId = 1 };


       await Assert.ThrowsAsync<ArgumentException>(() =>
           dataAccess.CreateStoryAsync(invalidStory));
   }


   [Fact]
   public async Task GetStoryByIdAsync_ShouldThrow_WhenNotFound()
   {
       using var context = CreateContext();
       var dataAccess = new TabloidDataAccess(context);


       await Assert.ThrowsAsync<KeyNotFoundException>(() =>
           dataAccess.GetStoryByIdAsync(999));
   }


   [Fact]
   public async Task UpdateStoryAsync_ShouldUpdateFields()
   {
       using var context = CreateContext();
       var story = new Story { Title = "Old", Content = "Old", DepartmentId = 1 };
       context.Stories.Add(story);
       context.SaveChanges();


       var dataAccess = new TabloidDataAccess(context);
       var updated = new Story { Title = "New", Content = "New" };


       var result = await dataAccess.UpdateStoryAsync(story.Id, updated);


       result.Title.Should().Be("New");
       result.Content.Should().Be("New");
   }


   [Fact]
   public async Task DeleteStoryAsync_ShouldRemove_WhenExists()
   {
       using var context = CreateContext();
       var story = new Story { Title = "DeleteMe", Content = "Test content", DepartmentId = 1 };
       context.Stories.Add(story);
       context.SaveChanges();


       var dataAccess = new TabloidDataAccess(context);
       await dataAccess.DeleteStoryAsync(story.Id);


       (await context.Stories.CountAsync()).Should().Be(0);
   }


   [Fact]
   public async Task GetDepartmentsAsync_ShouldReturnDepartmentsWithStories()
   {
       using var context = CreateContext();
       var dept = new Department { Name = "IT", Stories = new List<Story>() };
       context.Departments.Add(dept);
       context.SaveChanges();


       var dataAccess = new TabloidDataAccess(context);
       var result = await dataAccess.GetDepartmentsAsync();


       result.Should().ContainSingle(d => d.Name == "IT");
   }
}


