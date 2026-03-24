#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace OCR_MATH_ASSISTANT.Services
{
    public static class AreaSelector
    {
        /// <summary>
        /// Permite al usuario seleccionar un área rectangular en la pantalla con el mouse.
        /// </summary>
        /// <returns>El rectángulo seleccionado o null si se cancela.</returns>
        public static Rectangle? SelectArea()
        {
            // Crear un formulario transparente para la selección
            var form = new Form
            {
                Text = "Selecciona área matemática - ESC para cancelar",
                BackColor = Color.Black,
                Opacity = 0.3,
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized,
                TopMost = true,
                Cursor = Cursors.Cross,
                ShowInTaskbar = false
            };

            Rectangle? selection = null;
            Point startPoint = Point.Empty;
            bool isSelecting = false;

            // Eventos del mouse
            form.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    startPoint = e.Location;
                    isSelecting = true;
                }
            };

            form.MouseMove += (sender, e) =>
            {
                if (isSelecting)
                {
                    // Actualizar el área de selección visual
                    form.Invalidate();
                }
            };

            form.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left && isSelecting)
                {
                    Point endPoint = e.Location;
                    selection = new Rectangle(
                        Math.Min(startPoint.X, endPoint.X),
                        Math.Min(startPoint.Y, endPoint.Y),
                        Math.Abs(endPoint.X - startPoint.X),
                        Math.Abs(endPoint.Y - startPoint.Y)
                    );
                    
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
            };

            form.Paint += (sender, e) =>
            {
                if (isSelecting)
                {
                    Point currentPoint = form.PointToClient(Cursor.Position);
                    Rectangle rect = new Rectangle(
                        Math.Min(startPoint.X, currentPoint.X),
                        Math.Min(startPoint.Y, currentPoint.Y),
                        Math.Abs(currentPoint.X - startPoint.X),
                        Math.Abs(currentPoint.Y - startPoint.Y)
                    );

                    // Dibujar rectángulo de selección
                    using (Pen pen = new Pen(Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(pen, rect);
                    }

                    // Rellenar con color semitransparente
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, Color.Red)))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
            };

            form.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    form.DialogResult = DialogResult.Cancel;
                    form.Close();
                }
            };

            // Mostrar el formulario y esperar selección
            if (form.ShowDialog() == DialogResult.OK)
            {
                return selection;
            }

            return null;
        }

        /// <summary>
        /// Captura una área específica de la pantalla.
        /// </summary>
        /// <param name="rect">El rectángulo a capturar.</param>
        /// <returns>Bitmap del área capturada.</returns>
        public static Bitmap CaptureArea(Rectangle rect)
        {
            Bitmap screenshot = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size);
            }
            return screenshot;
        }
    }
}
