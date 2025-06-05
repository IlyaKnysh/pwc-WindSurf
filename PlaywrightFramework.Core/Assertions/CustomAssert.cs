using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace PlaywrightFramework.Core.Assertions
{
    public class CustomAssert
    {
        public void IsTrue(bool condition, string message = null)
        {
            Assert.That(condition, Is.True, message);
        }

        public void IsFalse(bool condition, string message = null)
        {
            Assert.That(condition, Is.False, message);
        }

        public void AreEqual<T>(T expected, T actual, string message = null)
        {
            Assert.That(actual, Is.EqualTo(expected), message);
        }

        public void AreNotEqual<T>(T expected, T actual, string message = null)
        {
            Assert.That(actual, Is.Not.EqualTo(expected), message);
        }

        public void IsNull(object anObject, string message = null)
        {
            Assert.That(anObject, Is.Null, message);
        }

        public void IsNotNull(object anObject, string message = null)
        {
            Assert.That(anObject, Is.Not.Null, message);
        }

        public void Contains(string expected, string actual, string message = null)
        {
            Assert.That(actual, Does.Contain(expected), message);
        }

        public void DoesNotContain(string expected, string actual, string message = null)
        {
            Assert.That(actual, Does.Not.Contain(expected), message);
        }

        public void StartsWith(string expected, string actual, string message = null)
        {
            Assert.That(actual, Does.StartWith(expected), message);
        }

        public void EndsWith(string expected, string actual, string message = null)
        {
            Assert.That(actual, Does.EndWith(expected), message);
        }

        public void IsMatch(string pattern, string actual, string message = null)
        {
            Assert.That(actual, Does.Match(pattern), message);
        }

        public void IsNotMatch(string pattern, string actual, string message = null)
        {
            Assert.That(actual, Does.Not.Match(pattern), message);
        }

        public void IsEmpty<T>(IEnumerable<T> collection, string message = null)
        {
            Assert.That(collection, Is.Empty, message);
        }

        public void IsNotEmpty<T>(IEnumerable<T> collection, string message = null)
        {
            Assert.That(collection, Is.Not.Empty, message);
        }

        public void Contains<T>(IEnumerable<T> collection, T expected, string message = null)
        {
            Assert.That(collection, Does.Contain(expected), message);
        }

        public void DoesNotContain<T>(IEnumerable<T> collection, T expected, string message = null)
        {
            Assert.That(collection, Does.Not.Contain(expected), message);
        }

        public void Fail(string message = null)
        {
            Assert.Fail(message);
        }

        public void Pass(string message = null)
        {
            Assert.Pass(message);
        }

        public void Inconclusive(string message = null)
        {
            Assert.Inconclusive(message);
        }

        public void Ignore(string message = null)
        {
            Assert.Ignore(message);
        }

        public void Throws<T>(Delegate code, string message = null) where T : Exception
        {
            Assert.Throws<T>(() => code.DynamicInvoke(), message);
        }

        public void DoesNotThrow(Delegate code, string message = null)
        {
            try
            {
                code.DynamicInvoke();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
