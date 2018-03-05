﻿using GraphQL.SchemaGenerator.Tests.Helpers;
using GraphQL.SchemaGenerator.Tests.Mocks;
using GraphQL.SchemaGenerator.Tests.Schemas;
using Xunit;

namespace GraphQL.SchemaGenerator.Tests.Tests
{
    public class MutationTests
    {
        [Fact]
        public void BasicExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetData{
                    setData (request:4){
                        data
                    }
                }
            ";

            var expected = @"{
              setData: {
                data: 4
              }
            }";

           GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void BasicExample_WithEnums_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    setState (request:Open){
                        state
                    }
                }
            ";

            var expected = @"{
              setState: {
                state: ""Open""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }


        [Fact]
        public void BasicExample_WithQueryAndMutation_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    setState (request:Open){
                        state
                    }
                }
                query GetState{
                    getState{
                        state
                    }
                }
            ";

            var expected = @"{
              SetState:{setState: {
                state: ""Open""
              }},
             GetState:{getState: {
                state: ""Open""
              }
            }}";

            GraphAssert.QueryOperationsSuccess(schema, query, expected);
        }

        [Fact]
        public void BasicExample_WithDecimal_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    set(request:{decimal:24.15, data:2}){
                        decimal
                    }
                }
            ";

            var expected = @"{
              set: {
                decimal: 24.15
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public async void BasicExample_WithoutInt_Fails()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    set(request:{decimal:24.15}){
                        decimal
                    }
                }
            ";

            var result = await DocumentOperations.ExecuteOperationsAsync(schema, null, query);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public void SetAdvanced_WithSameQuery_WorksBackwardsCompatible()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    set:setAdvanced(request:{decimal:24.15, data:2}){
                        data
                        state
                    }
                }
            ";

            var expected = @"{
              set: {
                data: 2,
                state: ""Open""
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void AdvancedExample_WithEnums_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"
                mutation SetState{
                    set (request:{state:Open, data:2}){
                        state
                        data
                    }
                }
            ";

            var expected = @"{
              set: {
                state: ""Open"",
                data: 2
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        /// <summary>
        ///     Introspection was not working on Inputs due to a bug in GraphQl
        /// </summary>
        [Fact]
        public void AdvancedExample_Introspection_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(EchoStateSchema));

            var query = @"{
                __type(name:""Input_SetRequest""){
                        name
                        inputFields{
                            name
                            type{
                                kind
                                ofType{
                                    kind
                                }
                            }
                        }
                    }
                }
            ";

            var expected = @"{
              __type: {
                name: ""Input_SetRequest"",
                inputFields: [
                  {
                    name: ""data"",
                    type: {
                    kind: ""NON_NULL"",
                    ofType: {
                        kind: ""SCALAR""
                      }
                    }
                  },
                    {
                        name: ""decimal"",
                        type: {
                                kind: ""SCALAR"",
                          ofType: null
                        }
                    },
                  {
                    name: ""state"",
                    type: {
                      kind: ""ENUM"",
                      ofType: null
                    }
                  }
                ]
              }
            }";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

        [Fact]
        public void PrimitiveExample_Works()
        {
            var schemaGenerator = new SchemaGenerator(new MockServiceProvider());
            var schema = schemaGenerator.CreateSchema(typeof(PrimitiveSchema));

            var query = @"
                mutation Test{
                    testRequest (clear:true)
                }
            ";

            var expected = @"{testRequest:null}";

            GraphAssert.QuerySuccess(schema, query, expected);
        }

    
    }
}
