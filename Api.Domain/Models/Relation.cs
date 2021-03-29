namespace Api.Domain.Models
{
    public class Relation
    {
        #region snippet_Properties

        public string Entity { get; set; }

        public string LocalKey { get; set; }

        public string ForeignKey { get; set; }

        public bool JustOne { get; set; }

        #endregion

        #region snippet_Deconstruct

        public void Deconstruct(out string localKey, out string foreignKey, out bool justOne)
        {
            localKey = LocalKey;
            foreignKey = ForeignKey;
            justOne = JustOne;
        }

        #endregion
    }
}