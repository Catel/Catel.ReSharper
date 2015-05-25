// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITreeNodeExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ReSharper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using JetBrains.DocumentModel;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.Util;

    internal static class ITreeNodeExtensions
    {
        #region Public Methods and Operators

        private static Dictionary<string, List<DocumentRange>> GetDocumentRangesDictionary(this ITreeNode @this)
        {
            var fields = new Dictionary<string, List<DocumentRange>>();

            // NOTE: The expression 'field' pattern.
            const string FieldPattern = "/[*]([^*]+?)[*]/";
            var regex = new Regex(FieldPattern, RegexOptions.IgnorePatternWhitespace);
            string noteText = @this.GetText();
            MatchCollection commentMatchCollection = regex.Matches(noteText);
            foreach (Match match in commentMatchCollection)
            {
                string fieldName = match.Groups[1].Value;
                if (!fields.ContainsKey(fieldName))
                {
                    fields.Add(fieldName, new List<DocumentRange>());
                }

                int startOffSet = @this.GetTreeStartOffset().Offset + match.Index;
                int endOffSet = startOffSet + match.Length;

                const string TypeOfValueExpressionPattern = "typeof[(]\\s" + FieldPattern + "[)]";
                if (Regex.IsMatch(noteText, TypeOfValueExpressionPattern))
                {
                    // PATCH: When "typeof(/*Value*/)" is introduced the editor overreact and convert it to "typeof( /*Value*/)", note the awkward space, so...
                    startOffSet--;
                }

                fields[fieldName].Add(@this.GetDocumentRange().SetStartTo(startOffSet).SetEndTo(endOffSet));
            }

            return fields;
        }

#if R81 || R82 || R90
        public static Dictionary<string, List<DocumentRange>> GetFields(this ITreeNode @this)
        {
            Argument.IsNotNull(() => @this);

            return GetDocumentRangesDictionary(@this);
        }
#else
        public static Dictionary<string, List<TextRange>> GetFields(this ITreeNode @this)
        {
            Argument.IsNotNull(() => @this);
            var documentRangesDictionary = @this.GetDocumentRangesDictionary();
            
            var  fields = new Dictionary<string, List<TextRange>>();
            foreach (var keyValuePair in documentRangesDictionary)
            {
                fields[keyValuePair.Key] = keyValuePair.Value.Select(range => range.TextRange).ToList();
            }

            return fields;
        }
#endif

        #endregion
    }
}