using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UIElements;

namespace PropHunt.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private UIDocument mainMenu;

        private VisualElement m_RootMenu;

        private TextField m_ipAddress;
        private IntegerField m_port;

        private Button m_clientButton, m_hostButton;

        void Awake()
        {
            m_RootMenu = mainMenu.rootVisualElement;

            m_clientButton = m_RootMenu.Query<Button>("ClientButton");
            m_hostButton = m_RootMenu.Query<Button>("HostButton");

            m_ipAddress = m_RootMenu.Query<TextField>("IpField");
            m_port = m_RootMenu.Query<IntegerField>("PortField");
        }
        /// <summary>
        /// Use sanitized IP and Port to set up the connection.
        /// </summary>
        /// 
        private void Start()
        {
            m_clientButton.clicked += StartClient;
            m_hostButton.clicked += StartHost;
        }

        /// <summary>
        /// Starts the host using the given connection data.
        /// </summary>
        void StartHost()
        {
            SetUtpConnectionData();
            var result = NetworkManager.Singleton.StartHost();
            if (result)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Tests", UnityEngine.SceneManagement.LoadSceneMode.Single);
                return;
            }
        }


        /// <summary>
        /// Starts the Client using the given connection data.
        /// </summary>
        void StartClient()
        {
            SetUtpConnectionData();
            NetworkManager.Singleton.StartClient();
        }

        void SetUtpConnectionData()
        {
            var sanitizedIPText = SanitizeAlphaNumeric(m_ipAddress.text);
            var sanitizedPortText = SanitizeAlphaNumeric(m_port.text);

            ushort.TryParse(sanitizedPortText, out var port);

            var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(sanitizedIPText, port);
        }

        /// <summary>
        /// Sanitize user port InputField box allowing only alphanumerics and '.'
        /// </summary>
        /// <param name="dirtyString"> string to sanitize. </param>
        /// <returns> Sanitized text string. </returns>
        static string SanitizeAlphaNumeric(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
        }
    }
}