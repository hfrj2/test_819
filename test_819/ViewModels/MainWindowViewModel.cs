using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace test_819.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        // 当前进度值（0~100）
        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        // 是否正在执行（用于控制按钮可用状态）
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                // 通知命令重新评估是否可以执行
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        // 异步启动命令
        public DelegateCommand StartCommand { get; }

        public MainWindowViewModel()
        {
            // 创建异步命令，并绑定 CanExecute 条件：当不忙的时候才能点
            StartCommand = new DelegateCommand(ExecuteStartAsync)
                                ;
        }

        // 异步执行方法（点击按钮触发）
        private async void ExecuteStartAsync()
        {
            // 1. 标记忙碌状态（按钮自动变灰）
            IsBusy = true;
            ProgressValue = 0;

            try
            {
                // 2. 创建进度报告器（传入的 Lambda 会自动在 UI 线程执行）
                var progress = new Progress<int>(value =>
                {
                    // 这个回调一定运行在 UI 线程，放心更新 Binding
                    ProgressValue = value;
                });

                // 3. 执行耗时任务（放到后台线程，防止卡界面）
                await Task.Run(() => DoHeavyWork(progress));

                // 4. 完成提示
                MessageBox.Show("任务执行完毕！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出错啦：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 5. 恢复空闲状态（按钮恢复可点）
                IsBusy = false;
            }
        }

        // 模拟耗时工作（比如下载、计算）
        private void DoHeavyWork(IProgress<int> progress)
        {
            for (int i = 0; i <= 100; i++)
            {
                // 模拟耗时操作（100ms 一步，总共约 10 秒）
                System.Threading.Thread.Sleep(100);

                // 报告进度（这里是在后台线程调用，但 Progress 会自动封送到 UI 线程）
                progress.Report(i);
            }
        }
    }
}