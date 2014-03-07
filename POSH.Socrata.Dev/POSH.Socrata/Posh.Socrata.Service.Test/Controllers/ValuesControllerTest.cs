using Microsoft.VisualStudio.TestTools.UnitTesting;
using Posh.Socrata.Service.Controllers;

namespace Posh.Socrata.Service.Test.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Arrange
            CommentController controller = new CommentController();

            //// Act
            //IEnumerable<string> result = controller.Get();

            //// Assert
            //Assert.IsNotNull(result);
            //Assert.AreEqual(2, result.Count());
            //Assert.AreEqual("value1", result.ElementAt(0));
            //Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Arrange
            CommentController controller = new CommentController();
        }

        [TestMethod]
        public void Post()
        {
            // Arrange
            CommentController controller = new CommentController();

            // Act
            //controller.Post("value");

            // Assert
        }

        [TestMethod]
        public void Put()
        {
            // Arrange
            CommentController controller = new CommentController();

            // Act
            //controller.Put(5, "value");

            // Assert
        }

        [TestMethod]
        public void Delete()
        {
            // Arrange
            CommentController controller = new CommentController();

            // Assert
        }
    }
}