using System.ComponentModel.DataAnnotations;

namespace QuizApplication.Domain
{
    public class Answer
    {
        public int Id;

        /* Please leave this MaxLength attribute on the AnswerText property, 
         * the GUTS tests will fail if removed */
        [MaxLength(100)]
        public string AnswerText;

        public int QuestionId;
        public bool IsCorrect;
    }
}
