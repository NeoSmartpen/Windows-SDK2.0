using Neosmartpen.Net;
using System.Collections.Generic;
using System.Text;


namespace PenDemo.Models
{
    public class Page : List<Stroke>
    {
        /// <summary>
        /// Gets the Section Id of the NCode paper
        /// </summary>
        public int Section { get; private set; }

        /// <summary>
        /// Gets the Owner Id of the NCode paper
        /// </summary>
        public int Owner { get; private set; }

        /// <summary>
        /// Gets the Note Id of the NCode paper
        /// </summary>
        public int Note { get; private set; }

        /// <summary>
        /// Gets the Page Number of the NCode paper
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Adds a new stroke to the current Page object.
        /// </summary>
        /// <param name="stroke">Stroke object</param>
        public new void Add(Stroke stroke)
        {
            if (base.Count == 0)
            {
                this.Section = stroke.Section;
                this.Owner = stroke.Owner;
                this.Note = stroke.Note;
                this.PageNumber = stroke.Page;
            }
            base.Add(stroke);
        }

        /// <summary>
        /// Gets the JSON text of the Page
        /// </summary>
        public string ToJSON()
        {
            var sb = new StringBuilder();
            string indent1 = "  ";
            string indent2 = "    ";
            string indent3 = "      ";

            sb.AppendLine("{");

            // Page 메타데이터
            sb.AppendFormat("{0}\"section\": {1},\r\n", indent1, Section);
            sb.AppendFormat("{0}\"owner\": {1},\r\n", indent1, Owner);
            sb.AppendFormat("{0}\"note\": {1},\r\n", indent1, Note);
            sb.AppendFormat("{0}\"pageNumber\": {1},\r\n", indent1, PageNumber);

            // Strokes 배열 시작
            sb.AppendFormat("{0}\"strokes\": [\r\n", indent1);

            for (int i = 0; i < this.Count; i++)
            {
                var stroke = this[i];
                sb.AppendFormat("{0}{{\r\n", indent2);
                sb.AppendFormat("{0}\"color\": {1},\r\n", indent3, stroke.Color);
                sb.AppendFormat("{0}\"startTime\": {1},\r\n", indent3, stroke.TimeStart);
                sb.AppendFormat("{0}\"timeEnd\": {1},\r\n", indent3, stroke.TimeEnd);
                sb.AppendFormat("{0}\"dotCount\": {1},\r\n", indent3, stroke.Count);
                sb.AppendFormat("{0}\"dots\": \"{1}\"\r\n", indent3, stroke.DotsToBase64());
                sb.AppendFormat("{0}}}", indent2);

                if (i < this.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.AppendFormat("{0}]\r\n", indent1);
            sb.Append("}");

            return sb.ToString();
        }

    }
}
