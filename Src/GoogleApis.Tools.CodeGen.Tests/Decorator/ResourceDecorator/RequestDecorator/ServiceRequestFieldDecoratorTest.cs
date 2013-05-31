﻿/*
Copyright 2011 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.CodeDom;
using System.Collections.Generic;

using NUnit.Framework;

using Google.Apis.Discovery;
using Google.Apis.Testing;
using Google.Apis.Tools.CodeGen.Decorator.ResourceDecorator.RequestDecorator;

namespace Google.Apis.Tools.CodeGen.Tests.Decorator.ResourceDecorator.RequestDecorator
{
    /// <summary>
    /// Tests for the ServiceRequestField decorator
    /// </summary>
    [TestFixture]
    public class ServiceRequestFieldDecoratorTest
    {
        /// <summary>
        /// Tests the constructor of this decorator.
        /// </summary>
        [Test]
        public void ConstructTest()
        {
            Assert.DoesNotThrow(() => new ServiceRequestFieldDecorator());
        }

        /// <summary>
        /// Tests the DecorateClass method.
        /// </summary>
        [Test]
        public void DecorateClassTest()
        {
            var method = new MockMethod() { Name = "Method", Parameters = new Dictionary<string, IDiscoveryParameter>() };
            method.Parameters.Add("Param", new MockParameter() { Name = "Param" });
            var resource = new MockResource();
            resource.Methods.Add("Method", method);
            var resourceDecl = new CodeTypeDeclaration();
            var requestDecl = new CodeTypeDeclaration();

            // Confirm that the decorator has run correctly.
            var decorator = new ServiceRequestFieldDecorator();
            decorator.DecorateClass(resource, method, requestDecl, resourceDecl);
            Assert.AreEqual(3, requestDecl.Members.Count); // 3 properties (method name, http method and rest path)
            Assert.IsInstanceOf<CodeMemberProperty>(requestDecl.Members[0]);
            Assert.IsInstanceOf<CodeMemberProperty>(requestDecl.Members[1]);
            Assert.IsInstanceOf<CodeMemberProperty>(requestDecl.Members[2]);
            Assert.AreEqual("MethodName", requestDecl.Members[0].Name);
            Assert.AreEqual("HttpMethod", requestDecl.Members[1].Name);
            Assert.AreEqual("RestPath", requestDecl.Members[2].Name);
        }

        /// <summary>
        /// Confirms that the generated property is valid.
        /// </summary>
        [Test]
        public void GenerateImplementedStringConstantPropertyTest()
        {
            CodeMemberProperty property =
                ServiceRequestFieldDecorator.GenerateStringConstantPropertyOverride("Name", "Value");

            Assert.IsNotNull(property);
            Assert.AreEqual("Name", property.Name);
            Assert.IsTrue(property.HasGet);
            Assert.IsFalse(property.HasSet);
            Assert.IsTrue((property.Attributes & MemberAttributes.Public) == MemberAttributes.Public);
            Assert.AreEqual(1, property.GetStatements.Count);
            CodeExpression value = ((CodeMethodReturnStatement)property.GetStatements[0]).Expression;
            Assert.AreEqual("Value", ((CodePrimitiveExpression)value).Value);
        }
    }
}
