namespace Tree
{
    public class TreeUser{
        public static void main(){
            BinaryTree tree = new BinaryTree();
            tree.PrintInOrder();
            tree.PrintLevelWise();

            // tree.ConnectLevels();
            // tree.PrintInOrderSpecial();
        }
    }
}