using System;

/// <summary>
/// 软件版本号结构体
/// (符合软件版本号规范与命名原则,支持阶段标识和版本备注字段)
/// </summary>

//////////////////////
/// 常规的版本号定义，分三项：：<主版本号>.<次版本号>.<修订版本号>，如 1.0.0
///完全的版本号由四部分组成：第一部分为项目名称，第二部分为文件的描述，第三部分为当前软件的版本号，第四部分为文件阶段标识加文件后缀，例如：1.1.1.051021_beta
//////////////////////

[Serializable]
public struct VersionInfo
{
    public enum VersionTag
    {
        Null,
        BASE,
        ALPHA,
        BETA,
        RC,
        RELEASE
    }

    public static VersionInfo Create(string verStr)
    {
        string ver = string.Empty;
        VersionTag tag = VersionTag.Null;
        string rmk = string.Empty;
        if(verStr.IndexOf('_') > -1)
        {
            string[] bsp = verStr.Split('_');
            if (bsp != null && bsp.Length > 1)
            {
                ver = bsp[0];
                tag = (VersionTag)Enum.Parse(typeof(VersionTag), bsp[1].ToUpper());
                if(bsp.Length > 2)
                {
                    for (int i = 2; i < bsp.Length; i++)
                    {
                        if (i > 2) rmk += '_';
                        rmk += bsp[i];
                    }
                }
            }
        }
        else
        {
            ver = verStr;
        }
        
        if (ver.IndexOf('.') > -1)
        {
            string[] sp = ver.Split('.');
            if (sp != null && sp.Length > 2)
            {
                int v = int.Parse(sp[0]);
                int sv = int.Parse(sp[1]);
                int dv = int.Parse(sp[2]);
                int t = 0;
                if (sp.Length > 3)
                    t = int.Parse(sp[3]);

                return new VersionInfo(v, sv, dv, t, tag, rmk);
            }
        }
        return new VersionInfo();
    }

    public VersionInfo(int m, int s, int d)
    {
        this.main = m;
        this.sub = s;
        this.dev = d;

        this.time = 0;
        this.verTag = VersionTag.Null;
        this.remarks = string.Empty;
    }

    public VersionInfo(int m, int s, int d, int t)
    {
        this.main = m;
        this.sub = s;
        this.dev = d;
        this.time = t;

        this.verTag = VersionTag.Null;
        this.remarks = string.Empty;
    }

    public VersionInfo(int m, int s, int d, int t, VersionTag tag)
    {
        this.main = m;
        this.sub = s;
        this.dev = d;
        this.time = t;

        this.verTag = tag;
        this.remarks = string.Empty;

    }

    public VersionInfo(int m, int s, int d, int t, VersionTag tag, string remarks)
    {
        this.main = m;
        this.sub = s;
        this.dev = d;
        this.time = t;

        this.verTag = tag;
        this.remarks = remarks;
    }

    public int main;
    public int sub;
    public int dev;
    public int time;
    public VersionTag verTag;
    public string remarks; 
    public override string ToString()
    {
        return "VersionInfo<" + ToShortString() +">";
    }

    public string ToShortString()
    {
        return main + "." + sub + "." + dev
            + (time > 0 ? "." + time : "")
            + (verTag != VersionTag.Null ? "_" + verTag.ToString().ToLower() : "")
            + (string.IsNullOrEmpty(remarks) ? "" : "_" + remarks);
    }

    public string ToJSONString()
    {
        return "{\"main\":" + main + ",\"sub\":" + sub + ",\"dev\":" + dev + ",\"time\":" + time
            + ",\"verTag\":\"" + verTag.ToString().ToLower() + "\""
            + ",\"remarks\":\"" + remarks + "\""
            + "}";
    }

    /// <summary>
    /// 保持对象级比较
    /// </summary>
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// 提示remarks属性值和verTag不参与比较
    /// </summary>
    public static bool operator ==(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main == rhs.main && lhs.sub == rhs.sub && lhs.dev == rhs.dev && lhs.time == rhs.time;
    }
    public static bool operator !=(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main != rhs.main || lhs.sub != rhs.sub || lhs.dev != rhs.dev || lhs.time != rhs.time;
    }

    public static bool operator <(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main != rhs.main ? lhs.main < rhs.main 
            : lhs.sub != rhs.sub ? lhs.sub < rhs.sub 
            : lhs.dev != rhs.dev ? lhs.dev < rhs.dev
            : lhs.time < rhs.time;
    }

    public static bool operator >(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main != rhs.main ? lhs.main > rhs.main
            : lhs.sub != rhs.sub ? lhs.sub > rhs.sub
            : lhs.dev != rhs.dev ? lhs.dev > rhs.dev
            : lhs.time > rhs.time;
    }

    public static bool operator <=(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main != rhs.main ? lhs.main <= rhs.main
            : lhs.sub != rhs.sub ? lhs.sub <= rhs.sub
            : lhs.dev != rhs.dev ? lhs.dev <= rhs.dev
            : lhs.time <= rhs.time;
    }

    public static bool operator >=(VersionInfo lhs, VersionInfo rhs)
    {
        return lhs.main != rhs.main ? lhs.main >= rhs.main
            : lhs.sub != rhs.sub ? lhs.sub >= rhs.sub
            : lhs.dev != rhs.dev ? lhs.dev >= rhs.dev
            : lhs.time >= rhs.time;
    }

}
