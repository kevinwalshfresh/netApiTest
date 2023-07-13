﻿using System;
using BookApi.Controllers;
using BookApi.Data.Services;
using BookApi.Interfaces;
using BookApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.test
{
	public class BooksControllerTest
	{
        BookController _controller;
        IBookService _service;

        public BooksControllerTest()
		{
            _service = new BookService();
            _controller = new BookController(_service);
        }

        [Fact]
        public void GetAllTest()
        {
            //Arrange
            //Act
            var result = _controller.GetAllBooks();
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;

            Assert.IsType<List<Book>>(list.Value);



            var listBooks = list.Value as List<Book>;

            Assert.Equal(5, listBooks.Count);
        }

        [Theory]
        [InlineData("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200", "ab2bd817-98cd-4cf3-a80a-53ea0cd9c111")]
        public void GetBookByIdTest(string guid1, string guid2)
        {
            //Arrange
            var validGuid = new Guid(guid1);
            var invalidGuid = new Guid(guid2);

            //Act
            var notFoundResult = _controller.GetBook(invalidGuid);
            var okResult = _controller.GetBook(validGuid);

            //Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
            Assert.IsType<OkObjectResult>(okResult.Result);

            //Now we need to check the value of the result for the ok object result.
            var item = okResult.Result as OkObjectResult;

            //We Expect to return a single book
            Assert.IsType<Book>(item.Value);

            //Now, let us check the value itself.
            var bookItem = item.Value as Book;
            Assert.Equal(validGuid, bookItem.Id);
            Assert.Equal("Managing Oneself", bookItem.Title);
        }

        [Fact]
        public void AddBookTest()
        {
            //OK RESULT TEST START

            //Arrange
            var completeBook = new Book()
            {
                Author = "Author",
                Title = "Title",
                Description = "Description"
            };

            //Act
            var createdResponse = _controller.AddBook(completeBook);

            //Assert
            Assert.IsType<CreatedAtActionResult>(createdResponse);

            //value of the result
            var item = createdResponse as CreatedAtActionResult;
            Assert.IsType<Book>(item.Value);

            //check value of this book
            var bookItem = item.Value as Book;
            Assert.Equal(completeBook.Author, bookItem.Author);
            Assert.Equal(completeBook.Title, bookItem.Title);
            Assert.Equal(completeBook.Description, bookItem.Description);

            //OK RESULT TEST END

            //BADREQUEST AND MODELSTATE ERROR TEST START

            //Arrange
            var incompleteBook = new Book()
            {
                Author = "Author",
                Description = "Description"
            };

            //Act
            _controller.ModelState.AddModelError("Title", "Title is a requried filed");
            var badResponse = _controller.AddBook(incompleteBook);

            //Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);



            //BADREQUEST AND MODELSTATE ERROR TEST END
        }

        [Theory]
        [InlineData("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200", "ab2bd817-98cd-4cf3-a80a-53ea0cd9c111")]
        public void RemoveBookByIdTest(string guid1, string guid2)
        {
            //Arrange
            var validGuid = new Guid(guid1);
            var invalidGuid = new Guid(guid2);

            //Act
            var notFoundResult = _controller.Remove(invalidGuid);


            //Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.Equal(6, _service.GetAll().Count());

            //Act
            var okResult = _controller.Remove(validGuid);


            //Assert
            Assert.IsType<OkResult>(okResult);
            Assert.Equal(5, _service.GetAll().Count());
        }
    }
}

