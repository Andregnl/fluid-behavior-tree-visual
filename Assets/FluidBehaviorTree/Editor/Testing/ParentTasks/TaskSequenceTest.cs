﻿using Adnc.FluidBT.TaskParents;
using Adnc.FluidBT.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Adnc.FluidBT.Testing {
    public class TaskSequenceTest {
        private ITask _childA;
        private ITask _childB;
        private TaskSequence _sequence;

        [SetUp]
        public void SetupChild () {
            _sequence = new TaskSequence();

            _childA = Substitute.For<ITask>();
            _childA.Enabled.Returns(true);
            _childA.Update().Returns(TaskStatus.Success);
            _sequence.AddChild(_childA);

            _childB = Substitute.For<ITask>();
            _childB.Enabled.Returns(true);
            _childB.Update().Returns(TaskStatus.Success);
            _sequence.AddChild(_childB);
        }

        public class UpdateMethod : TaskSequenceTest {
            public class UpdateMethodMisc : UpdateMethod {
                [Test]
                public void Skips_nodes_marked_disabled () {
                    _childA.Enabled.Returns(false);

                    _sequence.Update();

                    _sequence.children.ForEach((child) => {
                        if (child.Enabled) {
                            child.Received(1).Update();
                        } else {
                            child.Received(0).Update();
                        }
                    });
                }

                [Test]
                public void It_should_run_update_on_all_success_child_tasks () {
                    _sequence.Update();

                    _sequence.children.ForEach((c) => c.Received(1).Update());
                }

                [Test]
                public void It_should_not_run_update_on_any_child_tasks_after_success () {
                    _sequence.Update();
                    _sequence.Update();

                    _sequence.children.ForEach((c) => c.Received(1).Update());
                }

                [Test]
                public void It_should_retick_a_continue_node_when_update_is_rerun () {
                    _childB.Update().Returns(TaskStatus.Continue);

                    _sequence.Update();
                    _sequence.Update();

                    _childB.Received(2).Update();
                }

                [Test]
                public void It_should_not_update_previous_nodes_when_a_continue_node_is_rerun () {
                    _childB.Update().Returns(TaskStatus.Continue);

                    _sequence.Update();
                    _sequence.Update();

                    _childA.Received(1).Update();
                }
            }

            public class WhenAbortSelf : UpdateMethod {
                [SetUp]
                public void SetAbortTypeSelf () {
                    _sequence.AbortType = AbortType.Self;
                }

                public class DefaultCalls : WhenAbortSelf {
                    [Test]
                    public void It_should_not_abort_on_the_1st_update_call () {
                        _sequence.Update();

                        _childA.Received(1).Update();
                    }

                    [Test]
                    public void It_should_not_fail_if_no_children_are_present () {
                        _sequence.children.Clear();

                        _sequence.Update();
                    }

                    [Test]
                    public void Nested_sequence_will_return_failure_while_being_ticked_through_multiple_frames () {
                        var parentSequence = new TaskSequence();
                        parentSequence.AddChild(_sequence);
                        _childB.Update().Returns(TaskStatus.Continue);

                        Assert.AreEqual(TaskStatus.Continue, parentSequence.Update());
                        _childA.Update().Returns(TaskStatus.Failure);

                        Assert.AreEqual(TaskStatus.Failure, parentSequence.Update());
                    }
                }

                public class WhenAbortIsReady : WhenAbortSelf {
                    [SetUp]
                    public void SetupAbort () {
                        _childB.Update().Returns(TaskStatus.Continue);
                        _sequence.Update();
                        _childA.Update().Returns(TaskStatus.Failure);
                    }

                    [Test]
                    public void Return_failure_on_tick_if_the_first_node_changes_from_success_to_failure () {
                        Assert.AreEqual(TaskStatus.Failure, _sequence.Update());
                    }

                    [Test]
                    public void Does_not_abort_if_not_marked_abort_self () {
                        _sequence.AbortType = AbortType.None;

                        Assert.AreEqual(TaskStatus.Continue, _sequence.Update());
                    }

                    [Test]
                    public void It_should_run_end_when_aborting_on_the_active_node () {
                        _sequence.Update();

                        _childB.Received(1).End();
                    }

                    [Test]
                    public void Triggers_reset_after_firing_abort () {
                        _sequence.Update();

                        _sequence.children.ForEach((child) => {
                            child.Received().Reset();
                        });
                    }
                }
            }

            public class ReturnedStatusType : UpdateMethod {
                [Test]
                public void Returns_success_if_no_children () {
                    _sequence.children.Clear();

                    Assert.AreEqual(TaskStatus.Success, _sequence.Update());
                }

                [Test]
                public void Should_be_success_if_all_child_tasks_pass () {
                    _sequence.Update();

                    Assert.AreEqual(TaskStatus.Success, _sequence.Update());
                }

                [Test]
                public void Should_be_failure_if_a_child_task_fails () {
                    _childA.Update().Returns(TaskStatus.Failure);

                    Assert.AreEqual(TaskStatus.Failure, _sequence.Update());
                }

                [Test]
                public void Should_be_continue_if_a_child_returns_continue () {
                    _childA.Update().Returns(TaskStatus.Continue);

                    Assert.AreEqual(TaskStatus.Continue, _sequence.Update());
                }
            }
        }

        public class ResetMethod : TaskSequenceTest {
            [Test]
            public void Resets_ticking_of_child_nodes () {
                _childB.Update().Returns(TaskStatus.Continue);

                _sequence.Update();
                _sequence.Reset();
                _sequence.Update();

                _sequence.children.ForEach((c) => c.Received(2).Update());
            }
        }
    }
}

