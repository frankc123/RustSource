﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using uLink;
using UnityEngine;

public static class NetErrorHelper
{
    private const int fixErrorSignageMask = 0xff;
    private const string kConnectFailServerSide = "Server failed to approve the connection ";
    private const int maxUserDefinedErrorCount = 0x77;
    private const int mostNegativeNoErrorValue = -5;
    private static readonly Dictionary<NetError, string> niceStrings = new Dictionary<NetError, string>(Enum.GetValues(typeof(NetError)).Length);
    private const int noErrorValue = 0;
    private const int userDefined1Value = 0x80;

    static NetErrorHelper()
    {
        CacheNiceStrings();
        NetworkConnectionError noError = NetworkConnectionError.NoError;
        IEnumerator enumerator = Enum.GetValues(typeof(NetworkConnectionError)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                if (((int) current) < noError)
                {
                    noError = (NetworkConnectionError) ((int) current);
                }
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
        if (noError != NetworkConnectionError.InternalDirectConnectFailed)
        {
            Debug.LogWarning(string.Concat(new object[] { "Most Negative Base ", noError, " (", (int) noError, ")" }));
        }
    }

    internal static NetworkConnectionError _uLink(this NetError error)
    {
        return (NetworkConnectionError) error;
    }

    private static string BuildNiceString(NetError value)
    {
        NetError error = value;
        switch (error)
        {
            case NetError.ProxyTargetNotConnected:
                return "Proxy target not connected";

            case NetError.ProxyTargetNotRegistered:
                return "Proxy target not registered";

            case NetError.ProxyServerNotEnabled:
                return "Proxy server not enabled";

            case NetError.ProxyServerOutOfPorts:
                return "Proxy server out of ports";

            case NetError.Facepunch_Kick_ServerRestarting:
                return "Server restarting";

            case NetError.Facepunch_Approval_Closed:
                return "Not accepting new connections.";

            case NetError.Facepunch_Approval_TooManyConnectedPlayersNow:
                return "Authorization busy";

            case NetError.Facepunch_Approval_ConnectorAuthorizeException:
                return "Server exception with authorization";

            case NetError.Facepunch_Approval_ConnectorAuthorizeExecution:
                return "Aborted starting of authorization";

            case NetError.Facepunch_Approval_ConnectorDidNothing:
                return "Server failed to start authorization";

            case NetError.Facepunch_Approval_ConnectorCreateFailure:
                return "Server was unable to start authorization";

            case NetError.Facepunch_Approval_ServerDoesNotSupportConnector:
                return "Unsupported ticket";

            case NetError.Facepunch_Approval_MissingServerManagement:
                return "Server is not prepared";

            case NetError.Facepunch_Approval_ServerLoginException:
                return "Server exception";

            case NetError.Facepunch_Approval_DisposedWait:
                return "Aborted authorization";

            case NetError.Facepunch_Approval_DisposedLimbo:
                return "Failed to run authorization";

            case NetError.Facepunch_Kick_MultipleConnections:
                return "Started a different connection";

            case NetError.Facepunch_Kick_Violation:
                return "Kicked because of violation";

            case NetError.Facepunch_Kick_RCON:
                return "Kicked by admin";

            case NetError.Facepunch_Kick_Ban:
                return "Kicked and Banned by admin";

            case NetError.Facepunch_Kick_BadName:
                return "Rejected name";

            case NetError.Facepunch_Connector_InLimboState:
                return "Lost connection during authorization";

            case NetError.Facepunch_Connector_WaitedLimbo:
                return "Server lost you while processing ticket";

            case NetError.Facepunch_Connector_RoutineMoveException:
                return "Server exception occured while awaiting authorization";

            case NetError.Facepunch_Connector_RoutineYieldException:
                return "Server exception occured when checking authorization";

            case NetError.Facepunch_Connector_MissingFeatureImplementation:
                return "Authorization produced an unhandled message";

            case NetError.Facepunch_Connector_Cancelled:
                return "A ticket was cancelled - try again";

            case NetError.Facepunch_Connector_AuthFailure:
                return "Authorization failed";

            case NetError.Facepunch_Connector_AuthException:
                return "Server exception while starting authorization";

            case NetError.Facepunch_Connector_MultipleAttempts:
                return "Multiple authorization attempts";

            case NetError.Facepunch_Connector_VAC_Banned:
                return "VAC banned";

            case NetError.Facepunch_Connector_AuthTimeout:
                return "Timed out authorizing your ticket";

            case NetError.Facepunch_Connector_Old:
                return "Ticket already used";

            case NetError.Facepunch_Connector_NoConnect:
                return "Lost authorization";

            case NetError.Facepunch_Connector_Invalid:
                return "Ticket invalid";

            case NetError.Facepunch_Connector_Expired:
                return "Ticket expired";

            case NetError.Facepunch_Connector_ConnectedElsewhere:
                return "Changed connection";

            case NetError.Facepunch_API_Failure:
                return "API Failure";

            case NetError.Facepunch_Whitelist_Failure:
                return "Not in whitelist";

            case NetError.NATTargetNotConnected:
                return "NAT target not connected";

            case NetError.NATTargetConnectionLost:
                return "NAT target connection lost";

            case NetError.NATPunchthroughFailed:
                return "NAT punchthrough";

            case NetError.IncompatibleVersions:
                return "Version incompatible";

            case NetError.ConnectionTimeout:
                return "Timed out";

            case NetError.LimitedPlayers:
                return "Server has limited players";

            case NetError.ConnectionFailed:
                return "Could not reach the server";

            case NetError.TooManyConnectedPlayers:
                return "Full";

            case NetError.RSAPublicKeyMismatch:
                return "RSA public key mismatch";

            case NetError.ConnectionBanned:
                return "Banned from connecting";

            case NetError.InvalidPassword:
                return "Invalid password";

            case NetError.DetectedDuplicatePlayerID:
                return "Duplicate players identified";
        }
        switch ((error + 5))
        {
            case NetError.NoError:
                return "Direct connect failed";

            case ~NetError.CreateSocketOrThreadFailure:
                return "Invalid server";

            case ~NetError.IncorrectParameters:
                return "Incorrect parameters";

            case ~NetError.EmptyConnectTarget:
                return "Could not create socket or thread";

            case ~NetError.InternalDirectConnectFailed:
                return "Already connected to different server";

            case ((NetError) 5):
                return null;
        }
        switch (error)
        {
            case NetError.IsAuthoritativeServer:
                return "Authoritative server";

            case NetError.ApprovalDenied:
                return "You've been denied from connecting";
        }
        return null;
    }

    private static void CacheNiceStrings()
    {
        IEnumerator enumerator = Enum.GetValues(typeof(NetError)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                NetError error = (NetError) ((int) current);
                string str = BuildNiceString(error);
                if ((str == null) && (error != NetError.NoError))
                {
                    Debug.LogWarning("NetError." + current + " has no nice string");
                    str = FallbackNiceString(error);
                }
                niceStrings[error] = str;
            }
        }
        finally
        {
            IDisposable disposable = enumerator as IDisposable;
            if (disposable == null)
            {
            }
            disposable.Dispose();
        }
    }

    private static string FallbackNiceString(NetError error)
    {
        int num = (int) error;
        return (error.ToString().Replace("Facepunch_", string.Empty) + "(" + num.ToString("X") + ")").Replace("_", " ");
    }

    public static string NiceString(this NetError value)
    {
        string str;
        if (niceStrings.TryGetValue(value, out str))
        {
            return str;
        }
        return FallbackNiceString(value);
    }

    public static NetError ToNetError(this NetworkConnectionError error)
    {
        int num = (int) error;
        if ((num < -5) && ((num >> 7) == -1))
        {
            num &= 0xff;
        }
        return (NetError) num;
    }
}

