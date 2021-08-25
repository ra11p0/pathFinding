using System.Windows.Forms;
using DragControlNamespace;
using System.Drawing;
using System.Collections.Generic;

namespace pathFinding
{
    class Window : Form
    {
        private bool suspend = false;
        private List<Node> nodes = new List<Node>();
        private List<string> path = new List<string>();
        public Window()
        {
            Button findPathButton = new Button();
            findPathButton.Text = "Find path";
            findPathButton.Click += (s, a) =>
            {
                Form dialog = new Form();
                Button confirm = new Button() { Text = "confirm"};
                ComboBox beginRoute = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
                ComboBox endRoute = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
                List<string> nodeNames = new List<string>();
                nodes.ForEach(x => nodeNames.Add(x._name));
                nodeNames.Sort();
                beginRoute.Items.AddRange(nodeNames.ToArray());
                endRoute.Items.AddRange(nodeNames.ToArray());
                FlowLayoutPanel layout = new FlowLayoutPanel() {Dock = DockStyle.Fill};
                confirm.Click += (cs, ca) =>
                {
                    try
                    {
                        path.Clear();
                        path = PathManagement.Path.GetPath(beginRoute.Text, endRoute.Text, nodes);
                        suspend = false;
                        this.Refresh();
                    }
                    catch(System.FormatException ex) 
                    {
                        MessageBox.Show(ex.ToString());
                    }
                };
                layout.Controls.AddRange(new Control[] { new Label() {Text = "Find route from: " }, beginRoute, new Label() {Text = "to: " }, endRoute, confirm});
                dialog.Controls.Add(layout);
                dialog.Show();
            };
            Button newNodeButton = new Button() { Text = "New node", Left = findPathButton.Right };
            newNodeButton.Click += (s, a) =>
              {
                  Form dialog = new Form();
                  Button confirm = new Button() { Text = "confirm" };
                  TextBox name = new TextBox();
                  FlowLayoutPanel layout = new FlowLayoutPanel() { Dock = DockStyle.Fill };
                  ComboBox connectedWith = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
                  List<string> nodeNames = new List<string>();
                  nodes.ForEach(x => nodeNames.Add(x._name));
                  connectedWith.Items.AddRange(nodeNames.ToArray());
                  nodeNames.Sort();
                  confirm.Click += (cs, ca) =>
                  {
                      if (connectedWith.Text == "") PathManagement.AddPath(ref nodes, name.Text);
                      else PathManagement.AddPath(ref nodes, name.Text, connectedWith.Text);
                      DragControl newNodeDrag = new DragControl(this, nodes.Find(x => x._name == name.Text)._representation);
                      this.Controls.Add(nodes.Find(x=> x._name == name.Text)._representation);
                      nodes.Find(x => x._name == name.Text)._representation.BringToFront();
                      nodes.Find(x => x._name == name.Text)._representation.MouseDown += (s, a) => suspend = true;
                      nodes.Find(x => x._name == name.Text)._representation.MouseUp += (s, a) =>
                      {
                          suspend = false;
                          this.Refresh();
                      };
                      suspend = false;
                      this.Refresh();
                      dialog.Close();
                  };
                  layout.Controls.AddRange(new Control[] { new Label() { Text = "Name of new node: ", Width = 120 }, name, new Label() { Text = "Connected with: " }, connectedWith, confirm });
                  dialog.Controls.Add(layout);
                  dialog.Show();
              };
            Button connectTwoNodes = new Button() { Text = "Connect nodes", Left = newNodeButton.Right };
            connectTwoNodes.Click += (s, a) =>
            {
                Form dialog = new Form();
                Button confirm = new Button() { Text = "confirm" };
                ComboBox node = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
                FlowLayoutPanel layout = new FlowLayoutPanel() { Dock = DockStyle.Fill };
                ComboBox connectedWith = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList };
                List<string> nodeNames = new List<string>();
                nodes.ForEach(x => nodeNames.Add(x._name));
                connectedWith.Items.AddRange(nodeNames.ToArray());
                node.Items.AddRange(nodeNames.ToArray());
                nodeNames.Sort();
                confirm.Click += (cs, ca) =>
                {
                    PathManagement.AddPath(ref nodes, node.Text, connectedWith.Text);
                    suspend = false;
                    this.Refresh();
                };
                layout.Controls.AddRange(new Control[] { new Label() { Text = "Connect: "}, node, new Label() { Text = "with: " }, connectedWith, confirm });
                dialog.Controls.Add(layout);
                dialog.Show();
            };

            Width = 800;
            Height = 600;
            foreach (Node node in nodes)
            {
                DragControl dragNode = new DragControl(this, node._representation);
                node._representation.MouseDown += (s, a) => suspend = true;
                node._representation.MouseUp += (s, a) =>
                   {
                       suspend = false;
                       this.Refresh();
                   };
                this.Controls.Add(node._representation);
            }
            this.Paint += (s, a) =>
            {
                Pen pen = new Pen(Color.Black, 3);
                foreach (Node node in nodes)
                {
                    Point centerOfNode = new Point(node._representation.Left + node._representation.Width / 2, node._representation.Top + node._representation.Height / 2);
                    foreach(string name in node._nodes)
                    {
                        List<Node> connectedNodes =  nodes.FindAll(x => x._name == name);
                        foreach(Node connectedNode in connectedNodes)
                        {
                            Point centerOfConnectedNode = new Point(connectedNode._representation.Left + connectedNode._representation.Width / 2, connectedNode._representation.Top + connectedNode._representation.Height / 2);
                            DrawLine(a, centerOfConnectedNode, centerOfNode, pen);
                        }
                    }
                }
                pen.Color = Color.Red;
                foreach(string currentProcessedNodeName in path)
                {
                    try
                    {
                        if (nodes.Find(x => x._name == currentProcessedNodeName) == null) break;
                        var firstNodeRepresentation = nodes.Find(x => x._name == currentProcessedNodeName)._representation;
                        var secondNodeName = path[path.FindIndex(x => x == currentProcessedNodeName) + 1];
                        var secondNodeRepresentation = nodes.Find(x => x._name == secondNodeName)._representation;
                        Point firstNodePosition = new Point(firstNodeRepresentation.Left + firstNodeRepresentation.Width / 2, firstNodeRepresentation.Top + firstNodeRepresentation.Height / 2);
                        Point secondNodePosition = new Point(secondNodeRepresentation.Left + secondNodeRepresentation.Width / 2, secondNodeRepresentation.Top + secondNodeRepresentation.Height / 2);
                        DrawLine(a, firstNodePosition, secondNodePosition, pen);
                    }
                    catch(System.ArgumentOutOfRangeException ex)
                    {
                        break;
                    }
                        
                }
            };
            this.Controls.AddRange(new Control[] { findPathButton, newNodeButton, connectTwoNodes });
        }
        public void DrawLine(PaintEventArgs e, Point start, Point end, Pen pen)
        {
            if (suspend) return;
            e.Graphics.DrawLine(pen, start, end);
        }
    }
}
