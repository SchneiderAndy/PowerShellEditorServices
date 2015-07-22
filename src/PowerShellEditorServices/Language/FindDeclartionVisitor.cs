﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Management.Automation.Language;

namespace Microsoft.PowerShell.EditorServices.Language
{
    /// <summary>
    /// The vistor used to find the defintion of a symbol
    /// </summary>
    internal class FindDeclartionVisitor : AstVisitor
    {
        private SymbolReference symbolRef;

        public SymbolReference FoundDeclartion{ get; private set; }

        public FindDeclartionVisitor(SymbolReference symbolRef)
        {
            this.symbolRef = symbolRef;
        }

        /// <summary>
        /// Decides if the current function defintion is the right defition
        /// for the symbol being searched for. The defintion of the symbol will be a of type 
        /// SymbolType.Function and have the same name as the symbol
        /// </summary>
        /// <param name="functionDefinitionAst">A FunctionDefinitionAst in the script's AST</param>
        /// <returns>A descion to stop searching if the right FunctionDefinitionAst was found, 
        /// or a decision to continue if it wasn't found</returns>
        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst functionDefinitionAst)
        {
            int startColumnNumber =
                functionDefinitionAst.Extent.Text.IndexOf(
                    functionDefinitionAst.Name) + 1;

            IScriptExtent nameExtent = new ScriptExtent()
            {
                Text = functionDefinitionAst.Name,
                StartLineNumber = functionDefinitionAst.Extent.StartLineNumber,
                StartColumnNumber = startColumnNumber,
                EndColumnNumber = startColumnNumber + functionDefinitionAst.Name.Length
            };

            if (symbolRef.SymbolType.Equals(SymbolType.Function) &&
                 nameExtent.Text.Equals(symbolRef.ScriptRegion.Text))
            {
                this.FoundDeclartion =
                    new SymbolReference(
                        SymbolType.Function,
                        nameExtent,
                        string.Empty);

                return AstVisitAction.StopVisit;
            }

            return base.VisitFunctionDefinition(functionDefinitionAst);
        }

        /// <summary>
        /// Decides if the current variable expression is the right defition for 
        /// the symbol being searched for. The defintion of the symbol will be a of type 
        /// SymbolType.Variable and have the same name as the symbol
        /// </summary>
        /// <param name="variableExpressionAst">A FunctionDefinitionAst in the script's AST</param>
        /// <returns>A descion to stop searching if the right VariableExpressionAst was found, 
        /// or a decision to continue if it wasn't found</returns>
        public override AstVisitAction VisitVariableExpression(VariableExpressionAst variableExpressionAst)
        {
            if(symbolRef.SymbolType.Equals(SymbolType.Variable) &&
                variableExpressionAst.Extent.Text.Equals(symbolRef.SymbolName))
            {
                this.FoundDeclartion =
                    new SymbolReference(
                        SymbolType.Variable,
                        variableExpressionAst.Extent,
                        string.Empty);

                return AstVisitAction.StopVisit;
            }

            return AstVisitAction.Continue;
        }
    }
}