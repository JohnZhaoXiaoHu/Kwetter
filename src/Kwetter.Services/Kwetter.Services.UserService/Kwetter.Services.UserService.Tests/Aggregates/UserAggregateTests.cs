﻿using Kwetter.Services.Common.Domain.Events;
using Kwetter.Services.UserService.Domain.Exceptions;
using Kwetter.Services.UserService.Domain.AggregatesModel.UserAggregate;
using Kwetter.Services.UserService.Domain.AggregatesModel.UserAggregate.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kwetter.Services.UserService.Tests.Aggregates
{
    [TestClass]
    public class UserAggregateTests
    {
        [TestMethod]
        public void Should_Create_UserAggregate_With_All_Domain_Events()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string username = "Mario";
            string profileDescription = "Hello everyone!";
            
            // Act
            UserAggregate userAggregate = new(userId, username, profileDescription);

            // Assert
            List<Type> domainEvents = Assembly.GetAssembly(typeof(UserCreatedDomainEvent)).GetTypes().Where(t => t.BaseType == typeof(DomainEvent)).ToList();
            List<Type> userDomainEvents = userAggregate.DomainEvents.Select(t => t.GetType()).ToList();
            userDomainEvents.AddRange(userAggregate.Profile.DomainEvents.Select(t => t.GetType()));
            Assert.IsTrue(new HashSet<Type>(domainEvents).SetEquals(userDomainEvents));
        }

        [TestMethod]
        public void Should_Throw_UserDomainException_With_Empty_Username()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string username = "";
            string profileDescription = "Hello everyone!";
            
            // Act & Assert
            UserDomainException userDomainException = Assert.ThrowsException<UserDomainException>(() => new UserAggregate(userId, username, profileDescription));
            Assert.IsTrue(userDomainException.Message.Equals("The username is null, empty or contains only whitespaces."));
        }
    }
}
