using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace PokerBot
{
    public class TableRendererForm : Form, TableRenderer
	{
        // # public

        private List<RenderImage> renderImagesList = new List<RenderImage>();

        public static TableRendererForm newTableRendererForm()
        {
            // lock
            EventWaitHandle wait = new AutoResetEvent(false);

            // ui
            TableRendererForm form = null;
            Thread threadUi = new Thread(delegate()
            {
                form = new TableRendererForm();
                wait.Set();
                Application.Run(form);
            });
            threadUi.Start();
            wait.WaitOne();

            return form;
        }

        public Control Control { get { return this; } }

        public void renderImage(Image image, int x, int y)
        {
            renderImagesList.Add(new RenderImage(ImageTools.toBitmap(image), new Point(x, y)));
        }

        public void renderImage(Image image, Point point)
        {
            renderImagesList.Add(new RenderImage(ImageTools.toBitmap(image), point));
        }

        public void clearImages()
        {
            renderImagesList.Clear();
        }

        delegate void SetImagesDelegate(List<RenderImage> value);
        delegate void SetControlDelegate(List<TableControl> value);
        delegate void SetTableDelegate(Table value);
        delegate void SetTableLayoutDelegate(TableLayout value);
        delegate void SetSituationDelegate(Situation value);
        delegate void SetRuleDelegate(Rule value);
        delegate void SetDecisionDelegate(Decision value);
        delegate void Repaint();

        public void render(Table table, TableLayout layout)
        {
            render(table, layout, null, null, null, new List<TableControl>());
        }

        public void render(Table table, TableLayout layout, Situation situation, Rule rule, Decision decision, List<TableControl> controls)
        {
            if (IsHandleCreated)
            {
                BeginInvoke(new SetTableDelegate(setTable), new object[] { table });
                BeginInvoke(new SetTableLayoutDelegate(setTableLayout), new object[] { layout });
                BeginInvoke(new SetSituationDelegate(setSituation), new object[] { situation });
                BeginInvoke(new SetRuleDelegate(setRule), new object[] { rule });
                BeginInvoke(new SetDecisionDelegate(setDecision), new object[] { decision });
                BeginInvoke(new SetImagesDelegate(setImages), new object[] { renderImagesList });
                BeginInvoke(new SetControlDelegate(setControls), new object[] { controls });
                BeginInvoke(new Repaint(repaint), new Object[] { });
            }
        }

        // # internal

        private List<RenderImage> images = new List<RenderImage>();
        private Table table = new Table();
        private TableLayout layout;
        private Rule rule;
        private Decision decision;
        private Situation situation;
        private List<TableControl> controls = new List<TableControl>();

        private TableRendererForm()
        {
            BackColor = Color.White;
            Size = new Size(800, 800);
            DoubleBuffered = true;
            ResizeRedraw = true;
            Paint += new PaintEventHandler(OnPaint);
            Closed += delegate(object o, EventArgs e) { Environment.Exit(0); };
            CenterToScreen();
        }
						
		private void repaint()
		{
			this.Update();
			this.Refresh();
		}

        private void setControls(List<TableControl> controls)
        {
            lock (this)
            {
                this.controls.Clear();
                this.controls.AddRange(controls);
            }
        }

        private void setTableLayout(TableLayout layout)
        {
            lock (this)
            {
                this.layout = layout;
            }
        }

        private void setTable(Table table)
		{
			lock(this)
			{
				this.table = table;
			}
		}

        private void setSituation(Situation situation)
		{
			lock(this)
			{
				this.situation = situation;
			}
		}

        private void setRule(Rule rule)
		{
			lock(this)
			{
				this.rule = rule;
			}
		}

        private void setDecision(Decision decision)
        {
            lock (this)
            {
                this.decision = decision;
            }
        }

        private void setImages(List<RenderImage> images)
		{
			lock(this)
			{
				this.images.Clear();
				this.images.AddRange(images);
			}
		}

        private void OnPaint(object sender, PaintEventArgs e)
	    {
            Graphics g = e.Graphics;
            lock (this)
            {
                TableRendererBase.DrawTable(g, images, table, layout, situation, rule, decision, controls);
            }
	    }		
	}
}
