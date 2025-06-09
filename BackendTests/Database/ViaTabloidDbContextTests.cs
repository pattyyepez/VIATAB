using System.Linq;
using System.Threading.Tasks;
using Database;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;


public class ViaTabloidDbContextTests
{
   private DbContextOptions<ViaTabloidDbContext> CreateOptions()
   {
       return new DbContextOptionsBuilder<ViaTabloidDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid())
           .Options;
   }


   [Fact]
   public async Task ShouldCreateStory_WithDepartmentRelationship()
   {
       var options = CreateOptions();


       using (var context = new ViaTabloidDbContext(options))
       {
           var dept = new Department { Id = 10, Name = "QA" };
           context.Departments.Add(dept);
           context.SaveChanges();


           var story = new Story { Title = "Test Story", Content = "Test content", DepartmentId = 10 };
           context.Stories.Add(story);
           await context.SaveChangesAsync();
       }


       using (var context = new ViaTabloidDbContext(options))
       {
           var storyWithDept = await context.Stories.Include(s => s.Department).FirstAsync();
           storyWithDept.Department.Should().NotBeNull();
           storyWithDept.Department.Name.Should().Be("QA");
       }
   }


   [Fact]
   public async Task ShouldCascadeDeleteStory_WhenDepartmentDeleted()
   {
       var options = CreateOptions();


       using (var context = new ViaTabloidDbContext(options))
       {
           var dept = new Department { Id = 20, Name = "ToDelete" };
           var story = new Story { Title = "Child", Content = "Test content", DepartmentId = 20 };
           context.Departments.Add(dept);
           context.Stories.Add(story);
           await context.SaveChangesAsync();
       }


       using (var context = new ViaTabloidDbContext(options))
       {
           var department = await context.Departments.Include(d => d.Stories).FirstAsync(d => d.Id == 20);
           context.Departments.Remove(department);
           await context.SaveChangesAsync();
       }


       using (var context = new ViaTabloidDbContext(options))
       {
           var stories = await context.Stories.ToListAsync();
           stories.Should().BeEmpty(); // Cascade delete should have removed story
       }
   }
}

