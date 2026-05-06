using MedGyn.MedForce.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.AI.FormRecognizer;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using MedGyn.MedForce.Service.Contracts;

namespace MedGyn.MedForce.Service.Services
{
    public class AzureOrderAnalysisService : IOrderAnalysisService
    {
        string endpoint = "https://medgynorderaiservices.cognitiveservices.azure.com";
        string key = "5bfc924e7856483cb7189c9400a4c90b";

        DocumentAnalysisClient client;

        public AzureOrderAnalysisService()
        {
            
            AzureKeyCredential credential = new AzureKeyCredential(key);
            client = new DocumentAnalysisClient(new Uri(endpoint), credential);
        }

        public async Task<IDictionary<string,OrderAnalysisContract>> AnalizeOrder(string FileUrl)
        {
            Dictionary<string, OrderAnalysisContract> orderProperties = new Dictionary<string, OrderAnalysisContract>();  
            Uri orderUri = new Uri(FileUrl);

            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-invoice", orderUri);
            AnalyzeResult result = operation.Value;

            //iterate the results and populates list of field values
            for (int i = 0; i < result.Documents.Count; i++)
            {
                AnalyzedDocument document = result.Documents[i];
                foreach (string field in document.Fields.Keys)
                {
                    DocumentField documentField = document.Fields[field];

                    if (documentField.FieldType == DocumentFieldType.List)
                    {
                        IReadOnlyList<DocumentField> documentFields = documentField.Value.AsList();
                        List<Dictionary<string, OrderAnalysisContract>> items = new List<Dictionary<string, OrderAnalysisContract>>();
                        foreach (DocumentField listItem in documentFields)
                        {
                            if (listItem.FieldType == DocumentFieldType.Dictionary)
                            {
                                
                                Dictionary<string, OrderAnalysisContract> itemProperties = new Dictionary<string, OrderAnalysisContract>();
                                IReadOnlyDictionary<string, DocumentField> itemFields = listItem.Value.AsDictionary();
                                foreach (string itemField in itemFields.Keys)
                                {
                                    DocumentField itemDocumentField = itemFields[itemField];

                                    OrderAnalysisContract itemProperty = new OrderAnalysisContract(itemField, itemDocumentField);

                                    if(itemProperties.ContainsKey(itemField))
                                    {
                                        itemProperties[itemField] = itemProperty;
                                    }
                                    else
                                    {
                                        itemProperties.Add(itemField, itemProperty);
                                    }
                                }

                                items.Add(itemProperties);
                            }
                        }

                        OrderAnalysisContract orderProperty = new OrderAnalysisContract(field, documentField);
                        orderProperty.Items = items;
                        if(orderProperties.ContainsKey(field))
                        {
                            orderProperties[field] = orderProperty;
                        }
                        else
                        {
                            orderProperties.Add(field, orderProperty);
                        }

                    }
                    else
                    {
                        OrderAnalysisContract orderProperty = new OrderAnalysisContract(field, documentField);

                        if(orderProperties.ContainsKey(field))
                        {
                            orderProperties[field] = orderProperty;
                        }
                        else
                        {
                            orderProperties.Add(field, orderProperty);
                        }
                    }
                       
                }
            }

            return orderProperties;
        }
    }
}
