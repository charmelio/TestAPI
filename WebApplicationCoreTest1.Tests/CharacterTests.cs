using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using WebApplicationCoreTest1.Controllers;
using WebApplicationCoreTest1.Models;
using Newtonsoft.Json;

namespace WebApplicationCoreTest1.Tests
{
    public class CharacterTests
    {
        [Fact]
        public void CharacterLookup_NameFoundExact()
        {
            var expected = "thomas";
            var tc = new TestController();

            var result = (OkObjectResult)tc.CharacterLookup(expected);

            Assert.NotNull(result);
            var resChar = JsonConvert.DeserializeObject<Character>(result.Value.ToString());

            Assert.Equal(expected, resChar.Name);
        }

        [Fact]
        public void CharacterLookup_NameFoundSimiliar()
        {
            var expected = "charm";
            var tc = (OkObjectResult)new TestController().CharacterLookup(expected);

            Assert.NotNull(tc);
            Assert.Contains(expected, JsonConvert.DeserializeObject<Character>(tc.Value.ToString()).Name, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CharacterLookup_NameNotFound()
        {
            var expected = string.Empty;
            var result = ((BadRequestObjectResult)new TestController().CharacterLookup(expected)).Value.ToString();

            Assert.Equal(TestController.CharNotFound, result);
        }
    }
}