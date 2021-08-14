using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

public class MyLog4NetFileHelper
{
    private string DEFAULT_LOG_FILENAME = string.Format("application_log_{0}.log", DateTime.Now.ToString("yyyyMMMdd_hhmm"));
    Logger root;
    public MyLog4NetFileHelper()
    {

    }

    public void Init()
    {
        root = ((Hierarchy)LogManager.GetRepository()).Root;
        root.Repository.Configured = true;
    }

    #region Public Helper Methods
    #region Console Logging
    public virtual void AddConsoleLogging()
    {
        ConsoleAppender C = GetConsoleAppender();
        AddConsoleLogging(C);
    }

    public virtual void AddConsoleLogging(ConsoleAppender C)
    {
        root.AddAppender(C);
    }
    #endregion

    #region File Logging
    public virtual FileAppender AddFileLogging()
    {
        return AddFileLogging(DEFAULT_LOG_FILENAME);
    }

    public virtual FileAppender AddFileLogging(string sFileFullPath)
    {
        return AddFileLogging(sFileFullPath, log4net.Core.Level.All);
    }

    public virtual FileAppender AddFileLogging(string sFileFullPath, log4net.Core.Level threshold)
    {
        return AddFileLogging(sFileFullPath, threshold, true);
    }

    public virtual FileAppender AddFileLogging(string sFileFullPath, log4net.Core.Level threshold, bool bAppendfile)
    {
        FileAppender appender = GetFileAppender(sFileFullPath, threshold, bAppendfile);
        root.AddAppender(appender);
        return appender;
    }
    public virtual RollingFileAppender AddRollingFileLogging(string sFileFullPath, log4net.Core.Level threshold, bool bAppendfile)
    {
        RollingFileAppender appender = GetRollingFileAppender(sFileFullPath, threshold, bAppendfile);
        root.AddAppender(appender);
        return appender;
    }
    public virtual SmtpAppender AddSMTPLogging(string smtpHost, string From, string To, string CC, string subject, log4net.Core.Level threshhold)
    {
        SmtpAppender appender = GetSMTPAppender(smtpHost, From, To, CC, subject, threshhold);
        root.AddAppender(appender);
        return appender;
    }

    #endregion


    public IAppender GetLogAppender(string AppenderName)
    {
        AppenderCollection ac = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Appenders;

        foreach (log4net.Appender.IAppender appender in ac)
        {
            if (appender.Name == AppenderName)
            {
                return appender;
            }
        }

        return null;
    }

    public void CloseAppender(string AppenderName)
    {
        log4net.Appender.IAppender appender = GetLogAppender(AppenderName);
        CloseAppender(appender);
    }

    private void CloseAppender(log4net.Appender.IAppender appender)
    {
        appender.Close();
    }

    #endregion

    #region Private Methods

    private SmtpAppender GetSMTPAppender(string smtpHost, string From, string To, string CC, string subject, log4net.Core.Level threshhold)
    {
        SmtpAppender lAppender = new SmtpAppender();
        lAppender.Cc = CC;
        lAppender.To = To;
        lAppender.From = From;
        lAppender.SmtpHost = smtpHost;
        lAppender.Subject = subject;
        lAppender.BufferSize = 512;
        lAppender.Lossy = false;
        lAppender.Layout = new
        log4net.Layout.PatternLayout("%date{yyyy-MM-dd HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
        lAppender.Threshold = threshhold;
        lAppender.ActivateOptions();
        return lAppender;
    }

    private ConsoleAppender GetConsoleAppender()
    {
        ConsoleAppender lAppender = new ConsoleAppender();
        lAppender.Name = "Console";
        lAppender.Layout = new
        log4net.Layout.PatternLayout(" %message %n");
        lAppender.Threshold = log4net.Core.Level.All;
        lAppender.ActivateOptions();
        return lAppender;
    }
    /// <summary>
    /// DETAILED Logging 
    /// log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
    ///  
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="threshhold"></param>
    /// <returns></returns>
    private FileAppender GetFileAppender(string sFileName, log4net.Core.Level threshhold, bool bFileAppend)
    {
        FileAppender lAppender = new FileAppender();
        lAppender.Name = sFileName;
        lAppender.AppendToFile = bFileAppend;
        lAppender.File = sFileName;
        //lAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy-MM-dd HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
        lAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy-MM-dd HH:mm:ss} [%logger:%line] %level - %message%newline%exception");
        lAppender.Threshold = threshhold;
        lAppender.ActivateOptions();
        return lAppender;
    }

    /// <summary>
    /// Allows us to create at runtime a RollingFileAppender.
    /// </summary>
    /// <param name="sFileName"></param>
    /// <param name="threshhold"></param>
    /// <param name="bFileAppend"></param>
    /// <returns></returns>
    private RollingFileAppender GetRollingFileAppender(string sFileName, log4net.Core.Level threshhold, bool bFileAppend)
    {
        RollingFileAppender lAppender = new RollingFileAppender();
        lAppender.Name = sFileName;
        lAppender.AppendToFile = bFileAppend;
        lAppender.File = sFileName;
        // "%date{ABSOLUTE} [%logger] %level - %message%newline%exception"
        lAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy-MM-dd HH:mm:ss} [%logger:%line] %level - %message%newline%exception");
        //lAppender.Layout = new log4net.Layout.PatternLayout("%date{ABSOLUTE} [%logger] %level - %message%newline%exception");
        lAppender.Threshold = threshhold;
        lAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
        lAppender.MaximumFileSize = "10MB";
        lAppender.MaxSizeRollBackups = 5;
        lAppender.StaticLogFileName = true;
        lAppender.ActivateOptions();
        return lAppender;
    }

    //private FileAppender GetFileAppender(string sFileName)
    //{
    //    return GetFileAppender(sFileName, log4net.Core.Level.All,true);
    //}

    #endregion

    private void ConfigureLog(string sFileName)
    {


    }
}
