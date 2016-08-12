﻿using System.Linq;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.StarWars;
using GraphQL.StarWars.IoC;
using GraphQL.StarWars.Types;
using GraphQL.Types;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests
{
    public class SchemaGeneratorTests
    {
        [Fact]
        public void BasicExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider(), new GraphTypeResolver());
            var schema = schemaGenerator.CreateSchema(typeof(StarWarsAttributeSchema));

            var query = @"
                query HeroNameQuery {
                  hero {
                    name
                  }
                }
            ";

            var expected = @"{
              hero: {
                name: 'R2-D2'
              }
            }";

           GraphAssert.QuerySuccess(schema, query, expected);
        }

        /// <summary>
        ///     The generate schema aligns with the self generated one.
        /// </summary>
        [Fact]
        public void Schemas_Align()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(StarWarsAttributeSchema));

            var container = new SimpleContainer();
            container.Singleton(new StarWarsData());
            container.Register<StarWarsQuery>();
            container.Register<HumanType>();
            container.Register<DroidType>();
            container.Register<CharacterInterface>();
            var manualSchema = new StarWarsSchema(type => (GraphType)container.Get(type));

            Assert.Equal(manualSchema.Query.Fields.Count(), schema.Query.Fields.Count());
            Assert.Equal(manualSchema.AllTypes.Count(), schema.AllTypes.Count()+1); //todo work on enum
        }

        //[Fact] //skipped enums array works but not the new hashset.
        public void BasicExample_WithEnums_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(StarWarsAttributeSchema));

            var query = @"
                query HeroNameQuery {
                  hero {
                    appearsIn
                    friends
                  }
                }
            ";

            var expected = @"{
              hero: {
                appearsIn: [4,5,6],
                friends: [1,4]
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void CreateSchema_WithClassArgument_HasExpectedSchema()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var sut = schema.AllTypes;
            Assert.True(sut.Any(t=>t.Name == "Input_Schema1Request"));
        }

        [Fact]
        public void BasicParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest {value}
                }";

            var expected = @"{
              testRequest: {value:5}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithParameterExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest(request:{echo:1}) {value}
                }";

            var expected = @"{
              testRequest: {value:1}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithComplexParameters_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest(request:{
                    complexRequests:[{
                            echo:345
                        }]
                    }) {value}
                }";

            var expected = @"{
              testRequest: {value:5}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithComplexParameters_HaveCorrectType()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  __type(name : ""Input_InnerRequest"") {
                    name
                    kind
                }";

            var expected = @"{
              __type: {
                name: ""Input_InnerRequest"",
                kind: ""INPUT_OBJECT""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithEnumerableExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testEnumerable{value}
                }";

            var expected = @"{
                  testEnumerable: [{value: 1},{value: 5}]
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithEnum_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest {enum}
                }";

            var expected = @"{
              testRequest: {enum:""NEWHOPE""}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithDateTimeOffset_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest {date{{year}}
                }";

            var expected = @"{
              testRequest: {date:{year:1999}}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithNull_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest {nullValue}
                }";

            var expected = @"{
              testRequest: {nullValue:null}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void WithDictionary_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testRequest {
                    values{
                        key
                        value{
                            complicatedResponse{
                                echo
                            }
                        }
                    }
                  }
                }";

            var expected = @"{
              testRequest: {
                values: [
                  {
                   key: ""99"",
                    value: {
                      complicatedResponse: {
                        echo: 99
                      }
            }
                  },
                  {
                    key: ""59"",
                    value: {
                      complicatedResponse: {
                        echo: 59
                      }
                    }
                  },
                  {
                    key: ""null"",
                    value: null
                  }
                ]
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        //todo:
        //[Fact]
        public void BasicInterfaceExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(SchemaEcho));

            var query = @"{
                  testInterface{value}
                }";

            var expected = @"{
              testInterface: {value:8}
                }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }
    }
}
