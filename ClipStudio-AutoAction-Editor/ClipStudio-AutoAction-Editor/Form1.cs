using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

/** laf ファイルの構造(2016/07/01)
文字列の直前の１バイトは文字数

*ヘッダー
*ファイル先頭
01 01 01 08 typename 06 01 0d typeActionSet 01 03 01 0B
actionArrayH 4byte（アクション集合の終わりまでのオフセット；これ＋0x30が 01 0f applicationUuid の位置）
01 01 01 08 typename 06 00 01 1byte（アクションの数）


*アクション
49 4byte（アクション終了までのオフセット） 01 01 01 08 typename 06 01 0a typeAction 

*フッター
*applicationUuid
01 0f applicationUuid 06 01 $10桁-4桁-4桁-10桁

*actionSetName
01 0d actionSetName 06 01 1byte（文字数）nbyte（アクションセット名）
**/

namespace ClipStudio_AutoAction_Editor {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            Form1_SizeChanged(null, null);
        }

        private sealed class ActionSet {
            private string FilePath;
            public string ActionSetName { get; private set; }
            private Action[] Actions;
            private byte[] Header;
            private byte[] Footer;
            public struct Action {
                public string Name;
                public byte[] Data;
            }

            public ActionSet(string filePath) {
                this.FilePath = filePath;
            }

            private static readonly int HEADER_SIZE = 64;
            public void ReadHeader(byte[] b) {
                this.Header = new byte[HEADER_SIZE];
                Array.Copy(b, this.Header, HEADER_SIZE);

                this.Actions = new Action[this.Header[HEADER_SIZE-1]];
            }

            private static readonly int ACTION_SIZE_LOCATION_BYTE = 44;
            private static readonly int HEADER_SIZE_LOCATION_TO_END_OFFSET_BYTE = HEADER_SIZE - ACTION_SIZE_LOCATION_BYTE - 4;
            private void recalculateHeader() {
                if (this.Header == null || this.Actions == null) {
                    return;
                }

                // num actions
                this.Header[HEADER_SIZE - 1] = (byte)this.Actions.Length;

                // action size
                var actionSize = HEADER_SIZE_LOCATION_TO_END_OFFSET_BYTE;
                foreach(var a in this.Actions) {
                    actionSize += a.Data.Length;
                }

                // write action size
                var i0 = (byte)(actionSize & 0xff);
                var i1 = (byte)((actionSize>>8) & 0xff);
                var i2 = (byte)((actionSize >> 16) & 0xff);
                var i3 = (byte)((actionSize >> 24) & 0xff);
                Array.Copy(new byte[] { i3, i2, i1, i0 }, 0, this.Header, ACTION_SIZE_LOCATION_BYTE, 4);
            }

#region helper tools
            private bool isMatch(byte[] b, int index, byte[] searchData) {
                for (var i = 0; i < searchData.Length; ++i) {
                    if (b[index + i] != searchData[i]) {
                        return false;
                    }
                }
                return true;
            }
            private int findBinaryIndex(byte[] b, byte[] searchData, int start = 0) {
                for (var i = start; i < b.Length; ++i) {
                    if (isMatch(b, i, searchData)) {
                        return i;
                    }
                }
                throw new System.Exception(searchData + " not found");
            }

            private int findBinaryIndexFromEnd(byte[] b, byte[] searchData, int start = 0) {
                if (start == 0) {
                    start = b.Length - searchData.Length;
                }

                var i = start;
                while (i > 0) {
                    if (isMatch(b, i, searchData)) {
                        return i;
                    }
                    --i;
                }
                throw new System.Exception(searchData + " not found");
            }

            // [start,end)
            private string binaryToString(byte[] b, int start, int end) {
                var target = new byte[end - start];
                Array.Copy(b, start, target, 0, target.Length);
                return System.Text.Encoding.UTF8.GetString(target);
            }
            #endregion

            public void ReadFooter(byte[] data) {
                // read action set name
                var nameHeader = new byte[] { 0x01, 0x0d, 0x61, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x53, 0x65, 0x74, 0x4e, 0x61, 0x6d, 0x65, 0x06, 0x01 };
                var binaryStart = findBinaryIndexFromEnd(data, nameHeader);
                var start = binaryStart + nameHeader.Length + 1;
                var end = start + data[start - 1];
                this.ActionSetName = binaryToString(data, start, end);

                var nameBinary = new byte[end-binaryStart];
                Array.Copy(data, binaryStart, nameBinary, 0, nameBinary.Length);

                // read actionUuid
                var uuidHeader = new byte[] { 0x01, 0x0f, 0x61, 0x70, 0x70, 0x6c, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6f, 0x6e, 0x55, 0x75, 0x69, 0x64, 0x06, 0x01 };
                binaryStart = findBinaryIndexFromEnd(data, uuidHeader);
                start = binaryStart + uuidHeader.Length + 1;
                end = start + data[start - 1];

                var uuidBinary = new byte[end-binaryStart];
                Array.Copy(data, binaryStart, uuidBinary, 0, uuidBinary.Length);

                // copy to Footer
                this.Footer = new byte[nameBinary.Length + uuidBinary.Length];
                Array.Copy(nameBinary, this.Footer, nameBinary.Length);
                Array.Copy(uuidBinary, 0, this.Footer, nameBinary.Length, uuidBinary.Length);
            }

            // アクション読み込んだの次のインデックスを返す（次のアクションのインデックスではない）
            private int readAction(out Action action, byte[] data, int index) {
                var actionSize = BitConverter.ToInt32(new byte[] { data[index+4], data[index+3], data[index+2], data[index+1] }, 0);
                var end = index + 5 + actionSize;

                action.Data = new byte[end - index];
                Array.Copy(data, index, action.Data, 0, action.Data.Length);

                // get action name
                var nameHeader = new byte[] { 0x61, 0x63, 0x74, 0x69, 0x6f, 0x6e, 0x4e, 0x61, 0x6d, 0x65, 0x06, 0x01 };
                var binaryStart = findBinaryIndex(action.Data, nameHeader);
                var anStart = binaryStart + nameHeader.Length + 1;
                var anEnd = anStart + action.Data[anStart - 1];
                action.Name = binaryToString(action.Data, anStart, anEnd);

                return end;
            }
            // ヘッダが正しく読み込まれている必要がある
            public void ReadActions(byte[] data) {
                if(this.Header == null) {
                    return;
                }

                var index = 0;
                for( var i = 0; i < this.Actions.Length; ++i) {
                    this.Actions[i] = new Action();
                    index = findBinaryIndex(data, new byte[] { 0x01, 0x01, 0x01, 0x08, 0x74, 0x79, 0x70, 0x65, 0x6e, 0x61, 0x6d, 0x65, 0x06, 0x01, 0x0a, 0x74, 0x79, 0x70, 0x65, 0x41, 0x63, 0x74, 0x69, 0x6f, 0x6e }, index);
                    index = readAction(out this.Actions[i], data, index - 5);
                }
            }

            public void Save() {
                if (this.Actions == null || this.Header == null || this.Footer == null) {
                    return;
                }

                var outBinarySize = this.Header.Length + this.Footer.Length;
                foreach(var a in this.Actions) {
                    outBinarySize += a.Data.Length;
                }
                var outBinary = new byte[outBinarySize];

                // copy header
                recalculateHeader();
                Array.Copy(this.Header, outBinary, this.Header.Length);

                // copy actions
                var nextAddr = this.Header.Length;
                foreach(var a in this.Actions) {
                    Array.Copy(a.Data, 0, outBinary, nextAddr, a.Data.Length);
                    nextAddr += a.Data.Length;
                }

                // copy footer
                Array.Copy(this.Footer, 0, outBinary, nextAddr, this.Footer.Length);

                // write
                if (System.IO.File.Exists(this.FilePath)) {
                    int count = 0;
                    while (System.IO.File.Exists(this.FilePath + ".bak" + count.ToString())) {
                        ++count;
                    }
                    System.IO.File.Move(this.FilePath, this.FilePath + ".bak" + count.ToString());
                }
                using (var fs = new System.IO.FileStream(this.FilePath, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write)) {
                    using (var bf = new System.IO.BinaryWriter(fs)) {
                        bf.Write(outBinary);
                    }
                }
            }

            public void ListActions(ListBox l) {
                foreach (var action in this.Actions) {
                    l.Items.Add(action.Name);
                }
            }

            public void ReadAction(string filePath) {
                using (var fs = System.IO.File.OpenRead(filePath)) {
                    this.FilePath = filePath;
                    var data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);

                    ReadHeader(data);
                    ReadFooter(data);
                    ReadActions(data);
                }
            }

            public Action ExtractAction(string actionName) {
                if(this.Actions == null) {
                    throw new Exception("Actions is not initialized");
                }

                var index = 0;
                for(; index < this.Actions.Length; ++index) {
                    if (this.Actions[index].Name == actionName) {
                        break;
                    }
                }
                if (index == this.Actions.Length) {
                    throw new Exception("Action is not found");
                }

                var result = this.Actions[index];

                // fill a blank
                for(++index; index < this.Actions.Length; ++index) {
                    this.Actions[index - 1] = this.Actions[index];
                }

                return result;
            }

            public void InsertActionsToHead(Action[] actions) {
                if(actions == null || this.Actions == null) {
                    return;
                }

                var newActions = new Action[actions.Length + this.Actions.Length];
                var i = 0;
                for(; i < actions.Length; ++i) {
                    newActions[i] = actions[i];
                }
                for(; i < actions.Length+this.Actions.Length; ++i) {
                    newActions[i] = this.Actions[i - actions.Length];
                }
                this.Actions = newActions;
            }
        }
        private ActionSet[] actionData = new ActionSet[2];


        private void readLafFile(ActionSet ad, ListBox listbox, Label label, string filePath) {
            ad.ReadAction(filePath);
            label.Text = ad.ActionSetName;
            listbox.Items.Clear();
            ad.ListActions(listbox);
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e) {
            var filePath = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            this.actionData[0] = new ActionSet(filePath);
            readLafFile(this.actionData[0], this.listBox1, this.AALabel1, filePath);
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
            else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void listBox2_DragDrop(object sender, DragEventArgs e) {
            var filePath = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            this.actionData[1] = new ActionSet(filePath);
            readLafFile(this.actionData[1], this.listBox2, this.AALabel2, filePath);
        }

        private void listBox2_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
            else {
                e.Effect = DragDropEffects.None;
            }
        }

        private static readonly int LABEL_PADDING_TOP_PX = 10;
        private static readonly int WINDOW_PADDING_TOP_PX = 30;
        private static readonly int WINDOW_PADDING_WIDTH_PX = 20;
        private static readonly int WINDOW_MIDDLE_PADDING_PX = 100;
        private static readonly int MOVE_ACTION_BUTTON_WIDTH_PX = 60;
        private void Form1_SizeChanged(object sender, EventArgs e) {
            // listbox
            var client = this.ClientRectangle;
            var listbox_width = (client.Width - WINDOW_PADDING_WIDTH_PX * 2 - WINDOW_MIDDLE_PADDING_PX) / 2;
            var listbox_height = (client.Height - WINDOW_PADDING_WIDTH_PX * 3);
            this.listBox1.Left = WINDOW_PADDING_WIDTH_PX;
            this.listBox1.Top = this.listBox2.Top = WINDOW_PADDING_TOP_PX;
            this.listBox2.Left = listbox_width + WINDOW_PADDING_WIDTH_PX + WINDOW_MIDDLE_PADDING_PX;
            this.listBox1.Width = this.listBox2.Width = listbox_width;
            this.listBox1.Height = this.listBox2.Height = listbox_height;

            // label
            this.AALabel1.Left = this.listBox1.Left;
            this.AALabel2.Left = this.listBox2.Left;
            this.AALabel1.Top = this.AALabel2.Top = LABEL_PADDING_TOP_PX;

            // save button
            this.SaveButton1.Top = this.SaveButton2.Top = client.Height - WINDOW_PADDING_TOP_PX;
            this.SaveButton1.Left = this.AALabel1.Left;
            this.SaveButton2.Left = this.AALabel2.Left;
            this.SaveButton1.Width = this.SaveButton2.Width = this.listBox1.Width;

            // move action button
            var buttonMargin = (WINDOW_MIDDLE_PADDING_PX - MOVE_ACTION_BUTTON_WIDTH_PX)/2;
            this.buttonToLeft.Width = this.buttonToRight.Width = MOVE_ACTION_BUTTON_WIDTH_PX;
            this.buttonToLeft.Left = this.buttonToRight.Left = this.listBox1.Right + buttonMargin;
        }

        private void SaveButton1_Click(object sender, EventArgs e) {
            if(this.actionData[0] != null) {
                this.actionData[0].Save();
            }
        }

        private void SaveButton2_Click(object sender, EventArgs e) {
            if(this.actionData[1] != null) {
                this.actionData[1].Save();
            }
        }

        // リストボックスの要素を削除することに注意
        private ActionSet.Action[] getActionsFromListBox(ListBox l, ActionSet actions) {
            var outActions = new ActionSet.Action[l.SelectedItems.Count];

            for(var i=0; i < l.SelectedItems.Count; ++i) {
                outActions[i] = actions.ExtractAction((string)l.SelectedItems[i]);
            }

            // 頭から削除すると連続削除するときに問題が出るので後ろから削除する
            for(var i = l.SelectedIndices.Count-1; i >= 0; --i) {
                l.Items.RemoveAt(l.SelectedIndices[i]);
            }

            return outActions;
        }

        private void insertActions(ListBox tolist, ActionSet toSet, ActionSet.Action[] actions) {
            toSet.InsertActionsToHead(actions);
            for(var i = actions.Length-1; i >= 0; --i) {
                tolist.Items.Insert(0, actions[i].Name);
            }
        }

        private void buttonToLeft_Click(object sender, EventArgs e) {
            if(this.listBox2.SelectedItems.Count == 0 || this.actionData[0] == null) {
                return;
            }

            var actions = getActionsFromListBox(this.listBox2, this.actionData[1]);
            insertActions(this.listBox1, this.actionData[0], actions);
        }

        private void buttonToRight_Click(object sender, EventArgs e) {
            if (this.listBox1.SelectedItems.Count == 0 || this.actionData[1] == null) {
                return;
            }

            var actions = getActionsFromListBox(this.listBox1, this.actionData[0]);
            insertActions(this.listBox2, this.actionData[1], actions);
        }
    }
}
