using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
public static class StringExtension
{
    /// <summary>
    /// 移除前缀字符串
    /// </summary>
    /// <param name="self"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemovePrefixString(this string self, string str)
    {
        string strRegex = @"^(" + str + ")";
        return Regex.Replace(self, strRegex, "");
    }

    /// <summary>
    /// 移除后缀字符串
    /// </summary>
    /// <param name="self"></param>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveSuffixString(this string self, string str)
    {
        string strRegex = @"(" + str + ")" + "$";
        return Regex.Replace(self, strRegex, "");
    }

    /// <summary>
    /// 是否为Email
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsEmail(this string self)
    {
        return self.RegexMatch(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
    }

    /// <summary>
    /// 是否为域名
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsDomain(this string self)
    {
        return self.RegexMatch(@"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?");
    }

    /// <summary>
    /// 是否为IP地址
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsIP(this string self)
    {
        return self.RegexMatch(@"((?:(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d)\\.){3}(?:25[0-5]|2[0-4]\\d|[01]?\\d?\\d))");
    }

    /// <summary>
    /// 是否为手机号码
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsMobilePhone(this string self)
    {
        return self.RegexMatch(@"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
    }

    /// <summary>
    /// 是否为中文
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsChinese(this string self)
    {
        return self.RegexMatch("[\u4e00-\u9fa5]");
    }

    /// <summary>
    /// 匹配正则表达式
    /// </summary>
    /// <param name="slef"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static bool RegexMatch(this string slef, string pattern)
    {
        Regex reg = new Regex(pattern);
        return reg.Match(slef).Success;
    }

    /// <summary>
    /// 转换为MD5, 加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
    /// </summary>
    /// <param name="self"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static string ConvertToMD5(this string self, string flag = "x2")
    {
        byte[] sor = Encoding.UTF8.GetBytes(self);
        MD5 md5 = MD5.Create();
        byte[] result = md5.ComputeHash(sor);
        StringBuilder strbul = new StringBuilder(40);
        for (int i = 0; i < result.Length; i++)
        {
            strbul.Append(result[i].ToString(flag));
        }
        return strbul.ToString();
    }

    /// <summary>
    /// 转换为32位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_32(this string self)
    {
        return ConvertToMD5(self, "x2");
    }

    /// <summary>
    /// 转换为48位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_48(this string self)
    {
        return ConvertToMD5(self, "x3");
    }
    /// <summary>
    /// 转换为64位MD5
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static string ConvertToMD5_64(this string self)
    {
        return ConvertToMD5(self, "x4");
    }

    public static decimal ToDecimal(this string value)
    {
        return decimal.Parse(value);
    }

    public static decimal ToDecimal(this string value, decimal defaultValue)
    {
        var result = defaultValue;
        return decimal.TryParse(value, out result) ? result : defaultValue;
    }

    public static decimal ToRoundDecimal(this string value, decimal defaultValue, int decimals)
    {
        var result = defaultValue;
        result = System.Math.Round(decimal.TryParse(value, out result) ? result : defaultValue, decimals);
        return result;
    }

    public static decimal? ToNullableDecimal(this string value)
    {
        decimal result;
        if (string.IsNullOrEmpty(value) || !decimal.TryParse(value, out result))
        {
            return null;
        }
        return result;
    }
}
