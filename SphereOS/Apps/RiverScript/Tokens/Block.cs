namespace RiverScript.Tokens
{
    public class Block : ContainerToken
    {
        internal bool IsDescendantOf(Block block)
        {
            if (block.Parent == null)
                return false;

            if (block == this)
                return true;

            if (Parent is Block parentBlock)
            {
                return parentBlock.IsDescendantOf(block);
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "Block";
        }
    }
}
