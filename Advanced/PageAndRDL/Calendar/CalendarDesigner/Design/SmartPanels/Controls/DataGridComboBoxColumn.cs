using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using GrapeCity.ActiveReports.Design.DdrDesigner.Tools;
using GrapeCity.ActiveReports.Design.Resources;

namespace GrapeCity.ActiveReports.Design.Controls
{
	// https://stackoverflow.com/questions/41281920/datagridview-combobox-column-that-will-accept-any-text
	/// <summary>
	/// <see cref="DataGridViewColumn"/> implementation that shows combobox in the cell.
	/// This implementation differs from <see cref="DataGridViewComboBoxColumn"/> in the following:
	/// * you can enter any text in the cell
	/// * expression edit appeared when you choose '<Expression...>' option.
	/// </summary>
	internal class DataGridViewComboboxColumnEx : DataGridViewColumn
	{
		private readonly IDesignerHost _designerHost;

		public DataGridViewComboboxColumnEx(IServiceProvider serviceProvider) : base(new ComboboxCell())
		{
			_designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
		}

		public ComboBoxStyle DropDownStyle { get; set; }

		public object[] DropDownItems { get; set; }

		public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set => base.CellTemplate =
					value == null || value.GetType().IsAssignableFrom(typeof(ComboboxCell))
						? value
						: throw new InvalidCastException("Must be a ComboboxCell");
		}

		public static int ComboHeight = new ComboBox().Height + 3;

		public class ComboboxCell : DataGridViewTextBoxCell
		{
			public override void InitializeEditingControl(int rowIndex, object
				initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
			{
				// Set the value of the editing control to the current cell value.
				base.InitializeEditingControl(rowIndex, initialFormattedValue,
					dataGridViewCellStyle);
				var ctl = (ComboboxEditingControl)DataGridView.EditingControl;

				ctl.Text = (string)GetValue(rowIndex) ?? (string) DefaultNewRowValue;
			}

			public override Type EditType => typeof(ComboboxEditingControl);

			public override Type ValueType => typeof(string);

			public override object DefaultNewRowValue => null;

			public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle,
				TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
			{
				if ((string) formattedValue == string.Empty)
					return formattedValue;
				
				return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
			}
		}

		class ComboboxEditingControl : ComboBox, IDataGridViewEditingControl
		{
			DataGridView _dataGridView;

			public object EditingControlFormattedValue
			{
				get => Text;
				set => Text = value?.ToString() ?? string.Empty;
			}

			public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => 
				EditingControlFormattedValue;

			public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
			{
				Font = dataGridViewCellStyle.Font;
				ForeColor = dataGridViewCellStyle.ForeColor;
				BackColor = dataGridViewCellStyle.BackColor;
			}

			public int EditingControlRowIndex { get; set; }

			public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
			{
				// Let the DateTimePicker handle the keys listed.
				switch (key & Keys.KeyCode)
				{
					case Keys.Left:
					case Keys.Up:
					case Keys.Down:
					case Keys.Right:
					case Keys.Home:
					case Keys.End:
					case Keys.PageDown:
					case Keys.PageUp:
					case Keys.F4:
						return true;
					default:
						return !dataGridViewWantsInputKey;
				}
			}

			public void PrepareEditingControlForEdit(bool selectAll)
			{
				var col = _dataGridView.Columns[_dataGridView.CurrentCell.ColumnIndex] as DataGridViewComboboxColumnEx;
				if (col == null)
					throw new InvalidCastException("Must be in a DropDownComboBoxColumn");

				DropDownStyle = col.DropDownStyle;
				Items.Clear();
				Items.AddRange(col.DropDownItems);
				// If you don't explicitly set the Text then the current value is always
				// replaced with one from the drop-down list when edit begins.
				Text = CellText;
				SelectAll();
			}

			public bool RepositionEditingControlOnValueChange => false;

			public DataGridView EditingControlDataGridView
			{
				get => _dataGridView;
				set => _dataGridView = value;
			}

			public bool EditingControlValueChanged { get; set; } = false;

			public Cursor EditingPanelCursor => base.Cursor;

			private bool _recursionGuard = false;
			
			private bool IsValueExpressionSelected() => string.Equals(Text, Calendar.Resources.ValueExpression, StringComparison.OrdinalIgnoreCase);

			private bool IsValueBlankSelected() => string.Equals(Text, Calendar.Resources.ValueBlank, StringComparison.OrdinalIgnoreCase);

			private string CellText
			{
				get => _dataGridView.CurrentCell.Value?.ToString() ?? string.Empty;
				set
				{
					_dataGridView.CurrentCell.Value = value;
					_dataGridView.UpdateCellValue(_dataGridView.CurrentCell.ColumnIndex, _dataGridView.CurrentCell.RowIndex);
				}
			}

			/// <summary>
			/// Shows expression editor form when special of the dropdown value selected.
			/// </summary>
			protected override void OnSelectedIndexChanged(EventArgs e)
			{
				if (_recursionGuard) { return; }
				if (!Visible) { return; }
				_recursionGuard = true;
				try
				{
					var column =
						(DataGridViewComboboxColumnEx)_dataGridView.Columns[_dataGridView.CurrentCell.ColumnIndex];
				
					string value = Text;
					if (IsValueExpressionSelected())
					{
						var expressionForm = new ExpressionForm(column._designerHost) { Expression = CellText };

						var uiService = column._designerHost.GetService(typeof(IUIService)) as IUIService;
						value = uiService?.ShowDialog(expressionForm) == DialogResult.OK ? expressionForm.Expression : CellText;
						Focus();
					}
					else if (IsValueBlankSelected())
					{
						value = String.Empty;
					}

					if (value != null)
					{
						CellText = Text = value;
						EditingControlValueChanged = true;
						EditingControlDataGridView.NotifyCurrentCellDirty(true);
						
						//for some reason the selected value inside combobox is "<Expression...>" we have to update it
						Task.Delay(1).ContinueWith(_ =>
						{
							BeginInvoke((Action) (() =>
							{
								_recursionGuard = true;
								try
								{
									Text = value;
								}
								finally
								{
									_recursionGuard = false;
								}
							}));
						});
					}
				}
				finally
				{
					_recursionGuard = false;
				}
				base.OnSelectedIndexChanged(e);
			}

			protected override void OnTextChanged(EventArgs eventargs)
			{
				if (_recursionGuard) //when called from 'BeginInvoke' above
					return;
				
				EditingControlValueChanged = true;
				EditingControlDataGridView.NotifyCurrentCellDirty(true);
				base.OnTextChanged(eventargs);
			}
		}
	}
}

