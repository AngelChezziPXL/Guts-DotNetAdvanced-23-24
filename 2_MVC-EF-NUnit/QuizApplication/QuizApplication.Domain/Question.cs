using System.ComponentModel.DataAnnotations;

namespace QuizApplication.Domain
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionString;
        public int CategoryId;
        public IList<Answer> Answers;
    }
}
