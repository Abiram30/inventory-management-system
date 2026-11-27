using System;
using System.Collections.Generic;

namespace Inventory.Api.Models;

public partial class SystemSetting
{
    public int Id { get; set; }

    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }
}
