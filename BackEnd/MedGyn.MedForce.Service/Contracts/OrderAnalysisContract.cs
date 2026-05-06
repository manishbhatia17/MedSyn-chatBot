using Azure.AI.FormRecognizer.DocumentAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Service.Contracts
{
    public class OrderAnalysisContract
    {

        //Field name found on order
        public string Field { get; set; }
        //Field value
        public string Value { get; set; }
        //How confident AI is that field value is correct
        public string Score { get; set; }

        public List<Dictionary<string, OrderAnalysisContract>> Items { get; set; }

        public OrderAnalysisContract()
        {
            Field = "";
            Value = "";
            Score = "";
            Items = new List<Dictionary<string, OrderAnalysisContract>>();
        }
        public OrderAnalysisContract(string field, DocumentField documentField)
        {
            Field = field;
            Value = documentField.Content;
            Score = documentField.Confidence?.ToString();
        }

    }
}
