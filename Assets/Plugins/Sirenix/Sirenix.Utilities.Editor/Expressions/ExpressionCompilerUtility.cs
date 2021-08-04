#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ExpressionCompilerUtility.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ExpressionCompilerUtility.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Utilities.Editor.Expressions
{
    using System;

    /// <summary>
    /// Utility for compiling and emitting expression delegates.
    /// </summary>
    internal static class ExpressionCompilerUtility
    {
        private readonly static Tokenizer Tokenizer  = new Tokenizer();
        private readonly static ASTParser Parser     = new ASTParser(Tokenizer);
        private readonly static ASTEmitter Emitter   = new ASTEmitter();
        private readonly static EmitContext Context  = new EmitContext();

        /// <summary>
        /// Compiles an expression and tries to emit a delegate method.
        /// </summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted delegate if the expression is compiled successfully. Otherwise, null.</returns>
        public static Delegate CompileExpression(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.IsStatic = isStatic;
            Context.Type = contextType;
            Context.ReturnType = null;
            Context.Parameters = null;
            return CompileExpression(expression, Context, null, out errorMessage, richTextError);
        }

        /// <summary>
        /// Compiles an expression and tries to emit a delegate method.
        /// </summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="parameters">The parameters of the expression delegate.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted delegate if the expression is compiled successfully. Otherwise, null.</returns>
        public static Delegate CompileExpression(string expression, bool isStatic, Type contextType, Type[] parameters, out string errorMessage, bool richTextError = true)
        {
            Context.IsStatic = isStatic;
            Context.Type = contextType;
            Context.ReturnType = null;
            Context.Parameters = parameters;
            return CompileExpression(expression, Context, null, out errorMessage, richTextError);
        }

        /// <summary>
        /// Compiles an expression and tries to emit a delegate method.
        /// </summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="context">The emit context.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted delegate if the expression is compiled successfully. Otherwise, null.</returns>
        public static Delegate CompileExpression(string expression, EmitContext context, out string errorMessage, bool richTextError = true)
        {
            return CompileExpression(expression, context, null, out errorMessage, richTextError);
        }

        /// <summary>
        /// Compiles an expression and tries to emit a delegate of the specified type.
        /// </summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="context">The emit context.</param>
        /// <param name="delegateType">The type of the delegate to emit.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted delegate if the expression is compiled successfully. Otherwise, null.</returns>
        public static Delegate CompileExpression(string expression, EmitContext context, Type delegateType, out string errorMessage, bool richTextError = true)
        {
            errorMessage = null;
            try
            {
                Tokenizer.SetExpressionString(expression);
                return Emitter.EmitMethod("$Expression(" + expression + ")_" + Guid.NewGuid().ToString(), Parser.Parse(), context, delegateType);
            }
            catch (SyntaxException ex)
            {
                errorMessage = ex.GetNiceErrorMessage(expression, richTextError);
                return null;
            }
        }

        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<TResult> CompileFunc<TResult>(string expression, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = true;
            Context.Parameters = Type.EmptyTypes;
            return (ExpressionFunc<TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, TResult> CompileFunc<T1, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1) }
                : Type.EmptyTypes;
            return (ExpressionFunc<T1, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, TResult> CompileFunc<T1, T2, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2) }
                : new Type[] { typeof(T2) };
            return (ExpressionFunc<T1, T2, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, TResult> CompileFunc<T1, T2, T3, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3) }
                : new Type[] { typeof(T2), typeof(T3) };
            return (ExpressionFunc<T1, T2, T3, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, TResult> CompileFunc<T1, T2, T3, T4, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            return (ExpressionFunc<T1, T2, T3, T4, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, T5, TResult> CompileFunc<T1, T2, T3, T4, T5, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            return (ExpressionFunc<T1, T2, T3, T4, T5, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, T5, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, T5, T6, TResult> CompileFunc<T1, T2, T3, T4, T5, T6, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            return (ExpressionFunc<T1, T2, T3, T4, T5, T6, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, T5, T6, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, TResult> CompileFunc<T1, T2, T3, T4, T5, T6, T7, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            return (ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> CompileFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            return (ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionFunc method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionFunc if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> CompileFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(TResult);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            return (ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)CompileExpression(expression, Context, typeof(ExpressionFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>), out errorMessage, richTextError);
        }

        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction CompileAction(string expression, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = true;
            Context.Parameters = Type.EmptyTypes;
            return (ExpressionAction)CompileExpression(expression, Context, typeof(ExpressionAction), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1> CompileAction<T1>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1) }
                : Type.EmptyTypes;
            return (ExpressionAction<T1>)CompileExpression(expression, Context, typeof(ExpressionAction<T1>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2> CompileAction<T1, T2>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2) }
                : new Type[] { typeof(T2) };
            return (ExpressionAction<T1, T2>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3> CompileAction<T1, T2, T3>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3) }
                : new Type[] { typeof(T2), typeof(T3) };
            return (ExpressionAction<T1, T2, T3>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4> CompileAction<T1, T2, T3, T4>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4) };
            return (ExpressionAction<T1, T2, T3, T4>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4, T5> CompileAction<T1, T2, T3, T4, T5>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
            return (ExpressionAction<T1, T2, T3, T4, T5>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4, T5>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4, T5, T6> CompileAction<T1, T2, T3, T4, T5, T6>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
            return (ExpressionAction<T1, T2, T3, T4, T5, T6>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4, T5, T6>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4, T5, T6, T7> CompileAction<T1, T2, T3, T4, T5, T6, T7>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
            return (ExpressionAction<T1, T2, T3, T4, T5, T6, T7>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4, T5, T6, T7>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8> CompileAction<T1, T2, T3, T4, T5, T6, T7, T8>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
            return (ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8>), out errorMessage, richTextError);
        }
        /// <summary>Compiles an expression and emits an ExpressionAction method.</summary>
        /// <param name="expression">The expression to compile.</param>
        /// <param name="isStatic">Indicates if the expression should be static instead of instanced.</param>
        /// <param name="contextType">The context type for the execution of the expression.</param>
        /// <param name="errorMessage">Output for any errors that may occur.</param>
        /// <param name="richTextError">If <c>true</c> then error message will be formatted with color tags. Otherwise, the error message will be formatted with text only.</param>
        /// <returns>Returns the emitted ExpressionAction if the expression is compiled successfully. Otherwise, null.</returns>
        public static ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> CompileAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string expression, bool isStatic, Type contextType, out string errorMessage, bool richTextError = true)
        {
            Context.Type = contextType;
            Context.ReturnType = typeof(void);
            Context.IsStatic = isStatic || contextType.IsStatic();
            Context.Parameters = Context.IsStatic
                ? new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }
                : new Type[] { typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
            return (ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>)CompileExpression(expression, Context, typeof(ExpressionAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>), out errorMessage, richTextError);
        }
    }
}
#endif