using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GymMembers.Model;
using GymMembers.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GymMembers.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The list of registered members.
        /// </summary>
        private ObservableCollection<Member> members;

        /// <summary>
        /// The currently selected member.
        /// </summary>
        private Member selectedMember;

        /// <summary>
        /// The database that keeps track of saving and reading the registered members.
        /// </summary>
        private MemberDB database;


        /// <summary>
        /// The command that triggers adding a new member.
        /// </summary>
        public ICommand AddCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand ChangeCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            members = new ObservableCollection<Member>();
            database = new MemberDB(MemberList);
            members = database.GetMemberships();
            AddCommand = new RelayCommand(AddMethod);
            ExitCommand = new RelayCommand<IClosable>(ExitMethod);
            ChangeCommand = new RelayCommand(ChangeMethod);
            Messenger.Default.Register<MessageMember>(this, ReceiveMember);
            Messenger.Default.Register<NotificationMessage>(this, ReceiveMessage);
        }

        /// <summary>
        /// The currently selected member in the list box.
        /// </summary>
        public Member SelectedMember
        {
            get
            {
                return selectedMember;
            }
            set
            {
                selectedMember = value;
                RaisePropertyChanged("SelectedMember");
            }
        }

        /// <summary>
        /// Shows a new add screen.
        /// </summary>
        public void AddMethod()
        {
            AddWindow add = new AddWindow();
            add.Show();
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        /// <param name="window">The window to close.</param>
        public void ExitMethod(IClosable window)
        {
            if (window != null)
            {
                window.Close();
            }
                
        }

        /// <summary>
        /// Opens the change window.
        /// </summary>
        public void ChangeMethod()
        {
            if (SelectedMember != null)
            {
                ChangeWindow change = new ChangeWindow();
                change.Show();
                Messenger.Default.Send(SelectedMember);
            }
        }

        /// <summary>
        /// Gets a new member for the list.
        /// </summary>
        /// <param name="m">The member to add. The message denotes how it is added.
        /// "Update" replaces at the specified index, "Add" adds it to the list.</param>
        public void ReceiveMember(MessageMember m)
        {
            if (m.Message == "Update")
            {
                MemberList.Remove(SelectedMember);
                MemberList.Add(m);
                database.SaveMemberships();
            }
            else if (m.Message == "Add")
            {
                MemberList.Add(m);
                database.SaveMemberships();
            }
        }


        /// <summary>
        /// Gets text messages.
        /// </summary>
        /// <param name="msg">The received message. "Delete" means the currently selected member is deleted.</param>
        public void ReceiveMessage(NotificationMessage msg)
        {
            if (msg.Notification == "Delete")
            {
                MemberList.Remove(SelectedMember);
                database.SaveMemberships();
            }
        }

        /// <summary>
        /// The list of registered members.
        /// </summary>
        public ObservableCollection<Member> MemberList
        {
            get { return members; }
        }
    }
}