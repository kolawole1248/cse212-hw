public class Node
{
    public int Data { get; set; }
    public Node? Right { get; private set; }
    public Node? Left { get; private set; }

    public Node(int data)
    {
        this.Data = data;
    }

    public void Insert(int value)
    {
        // Check if the value already exists in the tree
        if (value == Data)
        {
            return; // Do nothing as the value already exists
        }

        if (value < Data)
        {
            // Insert to the left
            if (Left is null)
                Left = new Node(value);
            else
                Left.Insert(value);
        }
        else
        {
            // Insert to the right
            if (Right is null)
                Right = new Node(value);
            else
                Right.Insert(value);
        }
    }

    

    public bool Contains(int value)
    {
        // Base case: If the current node's value matches the search value
        if (value == Data)
        {
            return true;
        }

        // If the value is less than the current node's value, search in the left subtree
        if (value < Data)
        {
            if (Left is null)
            {
                return false; // Value is not found in the tree
            }
            return Left.Contains(value);
        }
        else
        {
            // If the value is greater than the current node's value, search in the right subtree
            if (Right is null)
            {
                return false; // Value is not found in the tree
            }
            return Right.Contains(value);
        }
    }

    public int GetHeight()
    {
        // Base Case: If the node is null, the height is 0
        if (this == null)
        {
            return 0;
        }

        // Recursive Case: Compute the height of the left and right subtrees
        int leftHeight = Left != null ? Left.GetHeight() : 0;
        int rightHeight = Right != null ? Right.GetHeight() : 0;

        // Return 1 plus the maximum height of the left and right subtrees
        return 1 + Math.Max(leftHeight, rightHeight);
    }
}